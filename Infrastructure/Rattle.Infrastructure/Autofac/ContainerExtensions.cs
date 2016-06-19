using Autofac;
using Rattle.Infrastructure;

namespace Rattle.Infrastruture.Autofac
{
    public static class ContainerExtensions
    {
        public static ContainerBuilder RegisterCommon(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MessageSerializer>().AsSelf().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<Publisher>().AsSelf().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<Consumer>().AsSelf().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<HandlerInvoker>().AsSelf().AsImplementedInterfaces().SingleInstance();

            return containerBuilder;
        }
    }
}
