using System.Configuration;
using Autofac;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using Rattle.Core.Commands;
using Rattle.Core.Events;
using Rattle.Core.Messages;
using Rattle.Infrastructure.Services.TopologyStrategies;
using Rattle.Infrastruture.Autofac;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rattle.Core.Requests;

namespace Rattle.Infrastructure.Services
{
    public abstract class Service
    {
        protected readonly string m_name;

        private ITopology m_topology;

        private IModel m_channel;
        private IPublisher m_publisher;
        private IConsumer<BasicDeliverEventArgs> m_consumer;
        private IMessageSerializer m_serializer;
        private IHandlerInvoker m_handlerInvoker;

        protected IContainer m_container;



        protected Service(string name)
        {
            m_name = name;
        }



        public async Task Start<TTopology>(IConnection connection) where TTopology : ITopology
        {
            await Task.Run(() =>
            {
                m_channel = connection.CreateModel();
                m_container = this.RegisterDependencies();

                m_publisher = m_container.Resolve<IPublisher>();
                m_consumer = m_container.Resolve<IConsumer<BasicDeliverEventArgs>>();

                m_topology = this.InitializeTopology<TTopology>();

                m_handlerInvoker = m_container.Resolve<IHandlerInvoker>();

                this.RegisterHandlersInInvoker();

                m_serializer = m_container.Resolve<IMessageSerializer>();
                m_serializer.KnownAssemblies = this.ContractsAssemblies.ToList();

                m_topology.Initialize()
                    .ListenForCommands(OnCommand)
                    .ListenForEvents(OnEvent);
            });
        }



        //protected void RegisterCommandHandler<TCommand>(ICommandHandler<TCommand> handler)
        //    where TCommand : ICommand
        //{
        //    m_handlerInvoker.RegisterHandler(handler);
        //}

        //protected void RegisterEventHanlder<TEvent>(IEventHandler<TEvent> handler)
        //    where TEvent : IEvent
        //{
        //    m_handlerInvoker.RegisterHandler(handler);
        //}




        protected abstract Assembly[] ContractsAssemblies { get; }

        protected abstract Assembly[] HandlersAssemblies { get; }

        protected abstract void RegisterDependencies(ContainerBuilder containerBuilder);



        private IContainer RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(m_channel).AsSelf().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterCommon();
            this.RegisterDependencies(containerBuilder);
            this.RegisterHandlersInContainer(containerBuilder);

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

                m_publisher.Publish("", deliveryArgs.BasicProperties.ReplyTo, response,
                    deliveryArgs.BasicProperties.CorrelationId);
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




        private void RegisterHandlersInContainer(ContainerBuilder containerBuilder)
        {
            foreach (var handlersAssembly in HandlersAssemblies)
            {
                var handlers = (from type in handlersAssembly.GetTypes()
                    from typeInterface in type.GetInterfaces()
                    where typeInterface.IsGenericType
                    let genericDefinition = typeInterface.GetGenericTypeDefinition()
                    where genericDefinition == typeof(ICommandHandler<>) ||
                          genericDefinition == typeof(IRequestHandler<>) ||
                          genericDefinition == typeof(IEventHandler<>)
                    select type).ToList();

                handlers.ForEach(
                    h => containerBuilder.RegisterType(h).AsSelf().AsImplementedInterfaces().SingleInstance());
            }
        }

        private void RegisterHandlersInInvoker()
        {
            foreach (var handlersAssembly in HandlersAssemblies)
            {
                var handlers = (from type in handlersAssembly.GetTypes()
                    from typeInterface in type.GetInterfaces()
                    where typeInterface.IsGenericType
                    let genericDefinition = typeInterface.GetGenericTypeDefinition()
                    where genericDefinition == typeof(ICommandHandler<>) ||
                          genericDefinition == typeof(IRequestHandler<>) ||
                          genericDefinition == typeof(IEventHandler<>)
                    select typeInterface).ToList();

                foreach (var handler in handlers)
                {
                    var actionType = handler.GetGenericArguments();
                    var handlerType = handler.GetGenericTypeDefinition();

                    var registerHandlerMethod = (from method in m_handlerInvoker.GetType().GetMethods()
                                                 where method.IsGenericMethod &&
                                                       method.Name == "RegisterHandler"
                                                 let methodParams = method.GetParameters()
                                                 where methodParams.Length == 1
                                                 let methodParam = methodParams[0]
                                                 where methodParam.ParameterType.Name == handlerType.Name &&
                                                       methodParam.ParameterType.Assembly == handlerType.Assembly
                                                 let genericMethod = method.MakeGenericMethod(actionType)
                                                 select genericMethod).Single();

                    registerHandlerMethod.Invoke(m_handlerInvoker, new[] {m_container.Resolve(handler)});
                }
            }
        }


        private ITopology InitializeTopology<TTopology>() where TTopology : ITopology
        {
            if (typeof(TTopology) == typeof(QueuePerMessageTopology))
            {
                return new QueuePerMessageTopology(this.m_name, this.HandlersAssemblies, this.m_channel, this.m_consumer);
            }

            if (typeof(TTopology) == typeof(QueuePerBusTopology))
            {
                return new QueuePerBusTopology(this.m_name, this.m_channel, this.m_consumer);
            }

            throw new ConfigurationErrorsException("Invalid topology type");
        }
    }
}
