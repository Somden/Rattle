using System;

namespace Rattle.Core.Bus
{
    public interface IConsumer<T>
    {
        void Consume(string queue, bool noAck, bool cancelOnReceive, Action<T> receiveHandler);
    }
}
