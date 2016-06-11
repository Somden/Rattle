using System;

namespace Rattle.Core.Messages
{
    public interface IMessageSerializer
    {
        byte[] Serialize<T>(T message) where T : IMessage;

        IMessage Deserialize(byte[] message);

        T Deserialize<T>(byte[] message) where T : IMessage;
    }
}
