using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Core.Events;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Services.TopologyStrategies;
using System;
using System.Diagnostics;
using System.Text;

namespace Rattle.Infrastructure.Services
{
    public class Service
    {
        private readonly ITopology m_topology;
        private readonly IModel m_channel;
        private readonly IPublisher m_publisher;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;
        private readonly IMessageSerializer m_serializer;
        private readonly IHandlerInvoker m_handlerInvoker;

        protected readonly string m_name;
        
        protected readonly string m_eventsQueue;


        public Service(string name, ITopology topology, IModel channel, IPublisher publisher,
            IConsumer<BasicDeliverEventArgs> consumer, IMessageSerializer serializer, IHandlerInvoker handlerInvoker)
        {
            m_name = name;
            m_eventsQueue = $"{m_name}_events";

            m_topology = topology;
            m_channel = channel;
            m_publisher = publisher;
            m_consumer = consumer;
            m_serializer = serializer;
            m_handlerInvoker = handlerInvoker;

            m_topology.Initialize(m_name)
                      .ListenForCommands(OnCommand)
                      .ListenForEvents(OnEvent);
        }



        public void RegisterCommandHandler<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : ICommand
        {
            m_handlerInvoker.RegisterHandler(handler);
        }

        public void RegisterEventHanlder<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            m_handlerInvoker.RegisterHandler(handler);
        }



        private void OnCommand(BasicDeliverEventArgs deliveryArgs)
        {
            if (!string.IsNullOrEmpty(deliveryArgs.BasicProperties.ReplyTo) &&
                !string.IsNullOrEmpty(deliveryArgs.BasicProperties.CorrelationId))
            {
                var command = m_serializer.Deserialize(deliveryArgs.Body) as ICommand;

                var response = m_handlerInvoker.Handle(command);

                m_publisher.Publish("", deliveryArgs.BasicProperties.ReplyTo, response, deliveryArgs.BasicProperties.CorrelationId);
            }
            else
            {
                Debug.Fail("Invalid data in delivery args");
            }
        }

        private void OnEvent(BasicDeliverEventArgs deliveryArgs)
        {
            var @event = m_serializer.Deserialize(deliveryArgs.Body) as IEvent;
            m_handlerInvoker.Handle(@event);
        }
    }
}
