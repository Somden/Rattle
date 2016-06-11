using System;
using Rattle.Core.Messages;

namespace Rattle.Core.Events
{
    public interface IEvent : IMessage
    {
        Guid Id { get; }
        int Version { get; }
    }
}

