using Rattle.Core.Bus;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace Rattle.Infrastructure
{
    public class Publisher : IPublisher
    {
        private readonly IModel m_channel;


        public Publisher(IModel channel)
        {
            m_channel = channel;
        }


        public void Publish<T>(string exchange, string topic, T message)
        {
            var properties = m_channel.CreateBasicProperties();

            this.Publish(exchange, topic, message, properties);
        }

        public void Publish<T>(string exchange, string topic, T message, string correlationId)
        {
            var properties = m_channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            this.Publish(exchange, topic, message, properties);
        }

        public void Publish<T>(string exchange, string topic, T message, string correlationId, string replyTo)
        {
            var properties = m_channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyTo;

            this.Publish(exchange, topic, message, properties);
        }



        private void Publish<T>(string exchange, string topic, T message, IBasicProperties properties)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            m_channel.BasicPublish(exchange, topic, properties, messageBuffer);
        }
    }
}
