using RabbitMQ.Client;
using Rattle.Core.Bus;
using Rattle.Infrastructure;
using System;

namespace Rattle.Server.Host.Temp
{
    class Program
    {
        private const string HOST = "localhost";
        private const string USERNAME = "guest";
        private const string PASSWORD = "guest";


        private static ICommandBus m_commandBus;


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

            var publisher = new Publisher(channel);
            var consumer = new Consumer(channel);

            m_commandBus = new CommandBus(channel, publisher, consumer);

            var service = new Service("test", channel, publisher, consumer);
            service.StartListener<string, string>(message => $"Re: {message}");
        }

        private static async void DoWork()
        {
            int i = 0;
            var initialMessage = "Hi";
            while (i < 5)
            {
                var response = await m_commandBus.Send<string, string>("test", initialMessage);
                Console.WriteLine($"Got response: {response}");
                initialMessage = response;
                i++;
            }
        }
    }
}
