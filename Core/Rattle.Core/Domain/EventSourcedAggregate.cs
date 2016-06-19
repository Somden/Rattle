using System.Collections.Generic;

namespace Rattle.Core.Domain
{
    public abstract class EventSourcedAggregate : Entity
    {
        public List<DomainEvent> Changes { get; private set; }
        public int Version { get; protected set; }

        protected EventSourcedAggregate()
        {
            Changes = new List<DomainEvent>();
        }

        public abstract void Apply(DomainEvent changes);
    }
}