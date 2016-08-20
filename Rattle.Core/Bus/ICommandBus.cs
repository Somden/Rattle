using Rattle.Core.Commands;
using Rattle.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Core.Bus
{
    public interface ICommandBus
    {
        Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command)
            where TCommand : ICommand
            where TResponse : IMessage;
    }
}
