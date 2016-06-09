using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Infrastructure
{
    public class CommandBus : ICommandBus
    {
        private readonly IModel m_channel;
        private readonly IPublisher m_publisher;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;

        public const string EXCHANGE_NAME = "CommandBus";

        public CommandBus(IModel channel, IPublisher publisher, IConsumer<BasicDeliverEventArgs> consumer)
        {
            m_channel = channel;
            m_publisher = publisher;
            m_consumer = consumer;

            m_channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, false);
        }



        public Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command)
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();

            var commandId = Guid.NewGuid().ToString();

            var responseQueue = m_channel.QueueDeclare().QueueName;

            m_consumer.Consume(responseQueue, true, true, deliveryArgs =>
            {
                if (deliveryArgs.BasicProperties != null &&
                    deliveryArgs.BasicProperties.CorrelationId == commandId)
                {
                    var responseJson = Encoding.UTF8.GetString(deliveryArgs.Body);
                    var response = JsonConvert.DeserializeObject<TResponse>(responseJson);
                    taskCompletionSource.SetResult(response);
                }
                else
                {
                    Debug.Fail("Invalid data in delivery args");
                }
            });

            m_publisher.Publish(EXCHANGE_NAME, service, command, commandId, responseQueue);

            return taskCompletionSource.Task;
        }
    }
}
