using RabbitMQ.Client;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Infrastructure;
using System;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Services;
using Rattle.Infrastructure.Services.TopologyStrategies;
using Rattle.Core.Events;

namespace Rattle.Server.Host.Temp
{
    #region Command

    class TextCommand : ICommand
    {
        public TextCommand(string text)
        {
            this.Text = text;
        }

        public string Text { get; private set; }
    }

    class TextCommandResponse : IMessage
    {
        public TextCommandResponse(string response)
        {
            this.Response = response;
        }

        public string Response { get; private set; }
    }

    class TextCommandHandler : ICommandHandler<TextCommand>
    {
        public IMessage Handle(TextCommand command)
        {
            return new TextCommandResponse($"Response to: {command.Text}");
        }
    }

    #endregion



    #region Event

    class TextEvent : IEvent
    {
        public TextEvent(string text)
        {
            this.AggregateId = Guid.NewGuid();
            this.Version = 1;
            this.Text = text;
        }

        public Guid AggregateId { get; private set; }

        public string AggregateType { get; private set; }

        public int Version { get; private set; }

        public string Text { get; private set; }
    }

    class TextEventHandler : IEventHandler<TextEvent>
    {
        public void Handle(TextEvent @event)
        {
            Console.WriteLine($"Handle event {@event.Version}: {@event.Text}");
        }
    }

    #endregion



    class Program
    {
        private const string HOST = "localhost";
        private const string USERNAME = "guest";
        private const string PASSWORD = "guest";


        private static ICommandBus m_commandBus;
        private static IEventBus m_eventBus;


        static void Main(string[] args)
        {
            Setup();
            DoWork();
            Console.ReadKey();
        }



        private static void Setup()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = HOST,
                UserName = USERNAME,
                Password = PASSWORD,
                Port = AmqpTcpEndpoint.UseDefaultPort,
                Protocol = Protocols.DefaultProtocol
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();


            var serializer = new MessageSerializer();
            serializer.KnownAssemblies.Add(typeof(TextCommand).Assembly);

            var publisher = new Publisher(channel, serializer);
            var consumer = new Consumer(channel, serializer);

            m_commandBus = new CommandBus(channel, publisher, consumer, serializer);
            m_eventBus = new EventBus(channel, publisher);

            var handlerInvoker = new HandlerInvoker();

            var topology1 = new QueuePerBusTopology("service1", channel, consumer);
            var service1 = new Service("service1", topology1, channel, publisher, consumer, serializer, handlerInvoker);
            service1.RegisterCommandHandler(new TextCommandHandler());
            service1.RegisterEventHanlder(new TextEventHandler());
            service1.Start();

            var topology2 = new QueuePerMessageTopology("service2", new[] { typeof(TextCommand).Assembly }, channel, consumer);
            var service2 = new Service("service2", topology2, channel, publisher, consumer, serializer, handlerInvoker);
            service2.RegisterCommandHandler(new TextCommandHandler());
            service2.RegisterEventHanlder(new TextEventHandler());
            service2.Start();
        }

        private static async void DoWork()
        {
            int i = 0;
            while (i < 5)
            {
                var response = await m_commandBus.Send<TextCommand, TextCommandResponse>($"service{i % 2 + 1}", new TextCommand("Hi"));
                m_eventBus.SendEvent<TextEvent>(new TextEvent("Hi from event"));
                Console.WriteLine(response.Response);
                i++;
            }
        }
    }
}
