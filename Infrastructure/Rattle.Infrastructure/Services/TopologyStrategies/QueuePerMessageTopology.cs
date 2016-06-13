using System;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Rattle.Core.Events;
using Rattle.Core.Commands;
using RabbitMQ.Client;
using Rattle.Core.Bus;

namespace Rattle.Infrastructure.Services.TopologyStrategies
{
    public class QueuePerMessageTopology : ITopology
    {
        private readonly string m_serviceName;
        private readonly Assembly[] m_handlersAssemblies;

        private readonly IModel m_channel;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;

        private readonly List<string> m_commandQueues = new List<string>();
        private readonly List<string> m_eventQueues = new List<string>();

        

        public QueuePerMessageTopology(string serviceName, Assembly[] handlersAssemblies, IModel channel, IConsumer<BasicDeliverEventArgs> consumer)
        {
            m_serviceName = serviceName;
            m_handlersAssemblies = handlersAssemblies;
            m_channel = channel;
            m_consumer = consumer;
        }



        public ITopology Initialize()
        {
            var eventTypes = this.GetImplementedHandlersTypes(typeof(IEventHandler<>));

            var commandTypes = this.GetImplementedHandlersTypes(typeof(ICommandHandler<>));

            foreach (var eventType in eventTypes)
            {
                var queueName = $"{m_serviceName}.events.{eventType.Name}";

                m_channel.QueueDeclare(queueName, true, false, false, null);
                m_channel.QueueBind(queueName, EventBus.EXCHANGE_NAME, $"event.*.{eventType.Name}");

                m_eventQueues.Add(queueName);
            }

            foreach (var commandType in commandTypes)
            {
                var queueName = $"{m_serviceName}.commands.{commandType.Name}";

                m_channel.QueueDeclare(queueName, true, false, false, null);
                m_channel.QueueBind(queueName, CommandBus.EXCHANGE_NAME, $"command.{m_serviceName}.{commandType.Name}");

                m_commandQueues.Add(queueName);
            }

            return this;
        }

        public ITopology ListenForCommands(Action<BasicDeliverEventArgs> onCommand)
        {
            foreach (var commandQueue in m_commandQueues)
            {
                m_consumer.Consume(commandQueue, false, false, onCommand);
            }

            return this;
        }

        public ITopology ListenForEvents(Action<BasicDeliverEventArgs> onEvent)
        {
            foreach (var eventQueue in m_eventQueues)
            {
                m_consumer.Consume(eventQueue, false, false, onEvent);
            }

            return this;
        }





        private List<Type> GetImplementedHandlersTypes(Type handler)
        {
            return (from assemby in m_handlersAssemblies
                    from type in assemby.GetTypes()
                    let eventHandlers = type.GetInterfaces().Where(i => i.IsGenericType &&
                                                                   i.GetGenericTypeDefinition() == handler)
                    where eventHandlers.Any()
                    from eventHandler in eventHandlers
                    select eventHandler.GetGenericArguments().First()).ToList();
        }
    }
}
