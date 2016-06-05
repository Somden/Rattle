using Rattle.Core.Messages;

namespace Rattle.Core.Commands
{
    public interface ICommand : IMessage
    {
        int ExpectedVersion { get; set; }
    }
}