using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using Rattle.Core.Messages;
using System;

namespace Rattle.Infrastructure.Services.TopologyStrategies
{
    public class QueuePerBusTopology : ITopology
    {
        private string m_eventsQueue;
        private string m_commandsQueue;

        private readonly IModel m_channel;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;
        private readonly IMessageSerializer m_serializer;

        public QueuePerBusTopology(IModel channel, IConsumer<BasicDeliverEventArgs> consumer, IMessageSerializer serializer)
        {
            m_channel = channel;
            m_consumer = consumer;
            m_serializer = serializer;
        }

        public ITopology Initialize(string serviceName)
        {
            m_eventsQueue = $"{serviceName}_events";
            m_commandsQueue = $"{serviceName}_commands";

            m_channel.QueueDeclare(m_eventsQueue, true, false, false, null);
            m_channel.QueueDeclare(m_commandsQueue, true, false, false, null);

            m_channel.QueueBind(m_eventsQueue, EventBus.EXCHANGE_NAME, string.Empty);
            m_channel.QueueBind(m_commandsQueue, CommandBus.EXCHANGE_NAME, serviceName);

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
