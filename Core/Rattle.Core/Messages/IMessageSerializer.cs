using System.Collections.Generic;
using System.Reflection;

namespace Rattle.Core.Messages
{
    public interface IMessageSerializer
    {
        List<Assembly> KnownAssemblies { get; set; }

        byte[] Serialize<T>(T message) where T : IMessage;

        IMessage Deserialize(string messageType, byte[] message);

        T Deserialize<T>(byte[] message) where T : IMessage;
    }
}
