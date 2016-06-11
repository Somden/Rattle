using System;
using Rattle.Core.Bus;
using Rattle.Core.Events;
using RabbitMQ.Client;

namespace Rattle.Infrastructure
{
    public class EventBus : IEventBus
    {
        public const string EXCHANGE_NAME = "EventBus";

        private readonly IModel m_channel;
        private readonly IPublisher m_publisher;


        public EventBus(IModel channel, IPublisher publisher)
        {
            m_channel = channel;
            m_publisher = publisher;

            m_channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Fanout, false);
        }

        
        public void SendEvent<T>(T @event) where T : IEvent
        {
            m_publisher.Publish(EXCHANGE_NAME, string.Empty, @event);
        }
    }
}
