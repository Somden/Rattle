using System;

namespace Rattle.Core.Events
{
    public interface IAggregateEvent : IEvent
    {
        Guid AggregateId { get; }
        string AggregateType { get; }
        int Version { get; }
    }
}
