using System;
using Rattle.Core.Messages;

namespace Rattle.Core.Events
{
    public interface IEvent : IMessage
    {
        Guid AggregateId { get; }
        string AggregateType { get; }
        int Version { get; }
    }
}

