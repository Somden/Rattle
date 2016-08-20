using Rattle.Core.Commands;
using Rattle.Core.Messages;
using System.Threading.Tasks;

namespace Rattle.Core.Bus
{
    public interface ICommandBus
    {
        Task<IMessage> Send<TCommand>(string service, TCommand command)
            where TCommand : ICommand;
    }
}
