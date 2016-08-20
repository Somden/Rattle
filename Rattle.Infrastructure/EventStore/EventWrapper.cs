using Rattle.Core.Events;

namespace Rattle.Infrastructure.EventStore
{
    public class EventWrapper
    {
        public string Id { get; private set; }
        public IAggregateEvent Event { get; private set; }
        public string EventStreamId { get; private set; }
        public int EventNumber { get; private set; }

        public EventWrapper(IAggregateEvent @event, int eventNumber, string streamStateId)
        {
            Event = @event;
            EventNumber = eventNumber;
            EventStreamId = streamStateId;
            Id = $"{streamStateId}-{EventNumber}";
        }
    }

    // used for persistence
}