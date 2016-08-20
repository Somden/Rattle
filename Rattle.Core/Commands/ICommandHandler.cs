using Rattle.Core.Messages;

namespace Rattle.Core.Commands
{
    public interface ICommandHandler<T> where T : ICommand
    {
        IMessage Handle(T command);
    }
}