using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using System;
using System.Diagnostics;
using System.Text;

namespace Rattle.Infrastructure
{
    public class Service
    {
        private readonly IModel m_channel;
        private readonly IPublisher m_publisher;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;

        protected readonly string m_name;

        public Service(string name, IModel channel, IPublisher publisher, IConsumer<BasicDeliverEventArgs> consumer)
        {
            m_name = name;
            m_channel = channel;
            m_publisher = publisher;
            m_consumer = consumer;

            m_channel.QueueDeclare(m_name, true, false, false, null);
            m_channel.QueueBind(m_name, CommandBus.EXCHANGE_NAME, m_name);
        }

        public void StartListener<TCommand, TResponse>(Func<TCommand, TResponse> handler)
        {
            m_consumer.Consume(m_name, false, false, deliveryArgs =>
            {
                if (!string.IsNullOrEmpty(deliveryArgs.BasicProperties.ReplyTo) &&
                    !string.IsNullOrEmpty(deliveryArgs.BasicProperties.CorrelationId))
                {
                    var commandJson = Encoding.UTF8.GetString(deliveryArgs.Body);
                    var command = JsonConvert.DeserializeObject<TCommand>(commandJson);

                    var response = handler(command);

                    m_publisher.Publish("", deliveryArgs.BasicProperties.ReplyTo, response, deliveryArgs.BasicProperties.CorrelationId);
                }
                else
                {
                    Debug.Fail("Invalid data in delivery args");
                }
            });
        }
    }
}
