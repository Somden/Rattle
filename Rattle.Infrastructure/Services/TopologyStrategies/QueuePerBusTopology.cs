using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using System;

namespace Rattle.Infrastructure.Services.TopologyStrategies
{
    public class QueuePerBusTopology : ITopology
    {
        private readonly string m_serviceName;

        private string m_eventsQueue;
        private string m_commandsQueue;

        private readonly IModel m_channel;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;

                

        public QueuePerBusTopology(string serviceName, IModel channel, IConsumer<BasicDeliverEventArgs> consumer)
        {
            m_serviceName = serviceName;
            m_channel = channel;
            m_consumer = consumer;
        }


        
        public ITopology Initialize()
        {
            m_eventsQueue = $"{m_serviceName}.events";
            m_commandsQueue = $"{m_serviceName}.commands";

            m_channel.QueueDeclare(m_eventsQueue, true, false, false, null);
            m_channel.QueueDeclare(m_commandsQueue, true, false, false, null);

            m_channel.QueueBind(m_eventsQueue, EventBus.EXCHANGE_NAME, "event.#");
            m_channel.QueueBind(m_commandsQueue, CommandBus.EXCHANGE_NAME, $"command.{m_serviceName}.*");

            return this;
        }

        public ITopology ListenForCommands(Action<BasicDeliverEventArgs> onCommand)
        {
            m_consumer.Consume(m_commandsQueue, false, false, onCommand);
            return this;
        }

        public ITopology ListenForEvents(Action<BasicDeliverEventArgs> onEvent)
        {
            m_consumer.Consume(m_eventsQueue, false, false, onEvent);
            return this;
        }
    }
}
