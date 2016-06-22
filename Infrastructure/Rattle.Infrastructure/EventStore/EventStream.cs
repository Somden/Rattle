using Rattle.Core.Domain;
using Rattle.Core.Events;

namespace Rattle.Infrastructure.EventStore
{
    public class EventStream
    {
        public string Id { get; set; } //aggregate type + id
        public int Version { get; set; }

        public EventStream() { }

        public EventStream(string id)
        {
            Id = id;
            Version = 0;
        }

        public EventWrapper RegisterEvent(IAggregateEvent @event)
        {
            Version++;

            return new EventWrapper(@event, Version, Id);
        }
    }
}