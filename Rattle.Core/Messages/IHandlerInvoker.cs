using Rattle.Core.Commands;
using Rattle.Core.Events;

namespace Rattle.Core.Messages
{
    public interface IHandlerInvoker
    {
        void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand;

        void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent;

        void Handle(IEvent @event);

        IMessage Handle(ICommand command);
    }
}
