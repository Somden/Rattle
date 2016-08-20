using Newtonsoft.Json;
using Rattle.Core.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rattle.Infrastructure
{
    public class MessageSerializer : IMessageSerializer
    {
        public List<Assembly> KnownAssemblies { get; set; } = new List<Assembly>();



        public byte[] Serialize<T>(T message) where T : IMessage
        {
            return Encoding.Default.GetBytes(JsonConvert.SerializeObject(message));
        }

        public IMessage Deserialize(string messageType, byte[] message)
        {
            var type = this.GetType(messageType);
            var messageJson = Encoding.Default.GetString(message);
            return JsonConvert.DeserializeObject(messageJson, type) as IMessage;
        }

        public T Deserialize<T>(byte[] message) where T : IMessage
        {
            var messageJson = Encoding.Default.GetString(message);
            return JsonConvert.DeserializeObject<T>(messageJson);
        }


        
        private Type GetType(string typeName)
        {
            foreach (var assembly in this.KnownAssemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new InvalidCastException("Serializer failed to find specified type.");
        }
    }
}
