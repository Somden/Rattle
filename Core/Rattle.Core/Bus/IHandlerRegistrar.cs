using System;
using Rattle.Core.Messages;

namespace Rattle.Core.Bus
{
    public interface IHandlerRegistrar
    {
        void RegisterHandler<T>(Action<T> handler) where T : IMessage;
    }
}