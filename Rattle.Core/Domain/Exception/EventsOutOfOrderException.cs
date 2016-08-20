using System;

namespace Rattle.Core.Domain.Exception
{
    public class EventsOutOfOrderException : System.Exception
    {
        public EventsOutOfOrderException(Guid id, int version)
            : base($"Eventstore gave event for aggregate {id} out of order. Event version: {version}")
        { }
    }
}