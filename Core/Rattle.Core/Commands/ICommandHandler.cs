using Rattle.Core.Messages;

namespace Rattle.Core.Commands
{
    public interface ICommandHandler<in T> : IHandler<T> where T : ICommand
    {
    }
}