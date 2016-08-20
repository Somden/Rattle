using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Exceptions;

namespace Rattle.Infrastructure
{
    public class CommandBus : ICommandBus
    {
        public const string EXCHANGE_NAME = "CommandBus";

        private readonly IModel m_channel;
        private readonly IPublisher m_publisher;
        private readonly IConsumer<BasicDeliverEventArgs> m_consumer;
        private readonly IMessageSerializer m_serializer;

        public CommandBus(IModel channel, IPublisher publisher, IConsumer<BasicDeliverEventArgs> consumer, IMessageSerializer serializer)
        {
            m_channel = channel;
            m_publisher = publisher;
            m_consumer = consumer;
            m_serializer = serializer;

            m_channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, true);
        }



        public Task<IMessage> Send<TCommand>(string service, TCommand command)
            where TCommand : ICommand
        {
            var taskCompletionSource = new TaskCompletionSource<IMessage>();

            var commandId = Guid.NewGuid().ToString();

            var responseQueue = m_channel.QueueDeclare().QueueName;

            m_consumer.Consume(responseQueue, true, true, deliveryArgs =>
            {
                if (deliveryArgs.BasicProperties != null &&
                    deliveryArgs.BasicProperties.CorrelationId == commandId &&
                    !string.IsNullOrEmpty(deliveryArgs.BasicProperties.Type))
                {
                    var response = m_serializer.Deserialize(deliveryArgs.BasicProperties.Type, deliveryArgs.Body);
                    taskCompletionSource.SetResult(response);
                }
                else
                {
                    taskCompletionSource.SetException(new DeliveryFailedException(deliveryArgs));
                }
            });

            var commandTopic = $"command.{service}.{command.GetType().Name}";
            m_publisher.Publish(EXCHANGE_NAME, commandTopic, command, commandId, responseQueue);

            return taskCompletionSource.Task;
        }
    }
}
