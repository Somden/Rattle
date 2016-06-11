using System;

namespace Rattle.Core.Bus
{
    public interface IConsumer<TNetworkMessage>
    {
        void Consume(string queue, bool noAck, bool cancelOnReceive, Action<TNetworkMessage> receiveHandler);
    }
}
