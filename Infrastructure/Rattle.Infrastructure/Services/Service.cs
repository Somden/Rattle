using Autofac;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Core.Events;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Services.TopologyStrategies;
using Rattle.Infrastruture.Autofac;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Infrastructure.Services
{
    public abstract class Service
    {
        private readonly IModel m_channel;

        private ITopology m_topology;
        private IPublisher m_publisher;
        private IConsumer<BasicDeliverEventArgs> m_consumer;
        private IMessageSerializer m_serializer;
        private IHandlerInvoker m_handlerInvoker;

        protected readonly string m_name;
        protected IContainer m_container;

        public Service(string name, IConnection connection)
        {
            m_name = name;
            m_channel = connection.CreateModel();
        }



        public async Task Start()
        {
            await Task.Run(() =>
            {
                m_container = this.RegisterDependencies();

                m_topology = m_container.Resolve<ITopology>();
                m_publisher = m_container.Resolve<IPublisher>();
                m_consumer = m_container.Resolve<IConsumer<BasicDeliverEventArgs>>();
                m_handlerInvoker = m_container.Resolve<IHandlerInvoker>();

                m_serializer = m_container.Resolve<IMessageSerializer>();
                m_serializer.KnownAssemblies = this.ContractsAssemblies.ToList();

                m_topology.Initialize()
                          .ListenForCommands(OnCommand)
                          .ListenForEvents(OnEvent);
            });
        }

        public void RegisterCommandHandler<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : ICommand
        {
            m_handlerInvoker.RegisterHandler(handler);
        }

        public void RegisterEventHanlder<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            m_handlerInvoker.RegisterHandler(handler);
        }




        protected abstract Assembly[] ContractsAssemblies { get; }

        protected abstract void RegisterTopology(ContainerBuilder containerBuilder);

        protected abstract void RegisterDependencies(ContainerBuilder containerBuilder);

        

        private IContainer RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(m_channel).AsSelf().AsImplementedInterfaces().SingleInstance();

            containerBuilder.RegisterCommon();
            this.RegisterTopology(containerBuilder);
            this.RegisterDependencies(containerBuilder);

            return containerBuilder.Build();
        }

        private void OnCommand(BasicDeliverEventArgs deliveryArgs)
        {
            if (!string.IsNullOrEmpty(deliveryArgs.BasicProperties.ReplyTo) &&
                !string.IsNullOrEmpty(deliveryArgs.BasicProperties.CorrelationId) &&
                !string.IsNullOrEmpty(deliveryArgs.BasicProperties.Type))
            {
                var command = m_serializer.Deserialize(deliveryArgs.BasicProperties.Type, deliveryArgs.Body) as ICommand;

                var response = m_handlerInvoker.Handle(command);

                m_publisher.Publish("", deliveryArgs.BasicProperties.ReplyTo, response, deliveryArgs.BasicProperties.CorrelationId);
            }
            else
            {
                Debug.Fail("Failed to deserialize command: Invalid data in delivery args");
            }
        }

        private void OnEvent(BasicDeliverEventArgs deliveryArgs)
        {
            if (!string.IsNullOrEmpty(deliveryArgs.BasicProperties.Type))
            {
                var @event = m_serializer.Deserialize(deliveryArgs.BasicProperties.Type, deliveryArgs.Body) as IEvent;
                m_handlerInvoker.Handle(@event);
            }
            else
            {
                Debug.Fail("Failed to deserialize event: Invalid data in delivery args");
            }
        }
    }
}
