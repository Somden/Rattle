using RabbitMQ.Client;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Infrastructure;
using System;
using System.CodeDom;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Services;
using Rattle.Infrastructure.Services.TopologyStrategies;
using Rattle.Core.Events;
using Autofac;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Rattle.Domain;
using Rattle.UserManagement;
using Rattle.UserManagement.Contracts.Commands;
using Rattle.UserManagement.Contracts.DTO;
using Rattle.UserManagement.Handlers.Commands;

namespace Rattle.Server.Host.Temp
{
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



        private static Task Setup()
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
            var publisher = new Publisher(channel, serializer);
            var consumer = new Consumer(channel, serializer);
            m_commandBus = new CommandBus(channel, publisher, consumer, serializer);
            m_eventBus = new EventBus(channel, publisher);

            var userManagementService = new UserManagementService(Services.UserManagement);
            return userManagementService.Start<QueuePerBusTopology>(connection);
        }

        private static async void DoWork()
        {
            int i = 0;
            while (i < 5)
            {
                var response = await m_commandBus.Send<RegisterUserCommand, UserDTO>(Services.UserManagement, new RegisterUserCommand("User1", "123"));
                Console.WriteLine($"User {response.Username} was successfully created");
                i++;
            }
        }
    }
}
