using Rattle.Core.Bus;
using RabbitMQ.Client;
using Rattle.Core.Messages;

namespace Rattle.Infrastructure
{
    public class Publisher : IPublisher
    {
        private readonly IModel m_channel;
        private readonly IMessageSerializer m_serializer;


        public Publisher(IModel channel, IMessageSerializer serializer)
        {
            m_channel = channel;
            m_serializer = serializer;
        }


        public void Publish<T>(string exchange, string topic, T message) where T : IMessage
        {
            var properties = m_channel.CreateBasicProperties();

            this.Publish(exchange, topic, message, properties);
        }

        public void Publish<T>(string exchange, string topic, T message, string correlationId) where T : IMessage
        {
            var properties = m_channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            this.Publish(exchange, topic, message, properties);
        }

        public void Publish<T>(string exchange, string topic, T message, string correlationId, string replyTo) where T : IMessage
        {
            var properties = m_channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyTo;

            this.Publish(exchange, topic, message, properties);
        }



        private void Publish<T>(string exchange, string topic, T message, IBasicProperties properties) where T : IMessage
        {
            properties.Type = typeof(T).FullName;
            var messageBuffer = m_serializer.Serialize(message);
            m_channel.BasicPublish(exchange, topic, properties, messageBuffer);
        }
    }
}
