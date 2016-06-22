using System;

namespace Rattle.Core.Events
{
    public interface IAggregateEvent : IEvent
    {
        Guid AggregateId { get; }
        int Version { get; }
    }
}
