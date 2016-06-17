using Rattle.Core.Domain.Exception;
using Rattle.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rattle.Core.Aggregates
{
    public abstract class Aggregate
    {
        private readonly List<IEvent> m_changes = new List<IEvent>();



        protected Aggregate()
        {
        }



        public Guid Id { get; protected set; }

        public int Version { get; protected set; }



        public bool IsDirty => m_changes.Any();
        


        public List<IEvent> GetUncommitedChanges()
        {
            return m_changes.ToList();
        }

        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                {
                    throw new EventsOutOfOrderException(e.AggregateId, e.Version);
                }

                ApplyEvent(e, false);
            }
        }

        public void MarkChangesAsCommited()
        {
            m_changes.Clear();
        }



        protected void ApplyEvent(IEvent @event)
        {
            ApplyEvent(@event, true);
        }

        protected void ApplyEvent(IEvent @event, bool isNew)
        {
            if (@event.AggregateId != this.Id)
            {
                return;
            }

            this.CallApplyMethod(@event);

            this.Id = @event.AggregateId;
            this.Version = @event.Version;

            if (isNew)
            {
                m_changes.Add(@event);
            }
        }

        protected void CallApplyMethod(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }
    }
}
