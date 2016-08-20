using Rattle.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Core.Bus
{
    public interface IPublisher
    {
        void Publish<T>(string exchange, string topic, T message) where T : IMessage;

        void Publish<T>(string exchange, string topic, T message, string correlationId) where T : IMessage;

        void Publish<T>(string exchange, string topic, T message, string correlationId, string replyTo) where T : IMessage;
    }
}
