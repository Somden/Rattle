using Rattle.Core.Messages;

namespace Rattle.Core.Commands
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        IMessage Handle(T command);
    }
}