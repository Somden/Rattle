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

            m_channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, true);
        }

        
        public void SendEvent<T>(T @event) where T : IEvent
        {
            var topic = this.GetTopic(@event);
            m_publisher.Publish(EXCHANGE_NAME, topic, @event);
        }

        private string GetTopic(IEvent @event)
        {
            if(@event is IAggregateEvent)
            {
                var aggregateEvent = @event as IAggregateEvent;
                return $"event.{aggregateEvent.AggregateType}.{aggregateEvent.GetType().Name}";
            }

            return $"event.{@event.GetType().Name}";
        }
    }
}
