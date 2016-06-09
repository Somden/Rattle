using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Core.Bus
{
    public interface ICommandBus
    {
        Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command);
    }
}
