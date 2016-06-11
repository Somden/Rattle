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
        #region Envelop

        private class Envelop
        {
            public Envelop()
            {
            }

            public Envelop(string type, string body)
            {
                this.Type = type;
                this.Body = body;
            }

            public string Type { get; set; }
            public string Body { get; set; }
        } 

        #endregion



        public List<Assembly> KnownAssemblies { get; set; } = new List<Assembly>();



        public byte[] Serialize<T>(T message) where T : IMessage
        {
            var envelop = new Envelop(message.GetType().FullName, JsonConvert.SerializeObject(message));
            return Encoding.Default.GetBytes(JsonConvert.SerializeObject(envelop));
        }

        public IMessage Deserialize(byte[] message)
        {
            var envelop = this.DeserializeEnvelop(message);

            var messageType = this.GetType(envelop.Type);

            return JsonConvert.DeserializeObject(envelop.Body, messageType) as IMessage;
        }

        public T Deserialize<T>(byte[] message) where T : IMessage
        {
            var envelop = this.DeserializeEnvelop(message);

            return JsonConvert.DeserializeObject<T>(envelop.Body);
        }




        private Envelop DeserializeEnvelop(byte[] message)
        {
            var envelopJson = Encoding.Default.GetString(message);
            return JsonConvert.DeserializeObject<Envelop>(envelopJson);
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
