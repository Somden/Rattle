using System;
using System.Collections.Generic;
using Rattle.Core.Aggregates;
using System.Linq;
using Rattle.Core.Domain;

namespace Rattle.Core.Data
{
    public abstract class AggregateRepository<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : Aggregate
    {
        private readonly Dictionary<Guid, TAggregate> m_aggregates = new Dictionary<Guid, TAggregate>();
        private readonly IEventStore m_eventStore;


        public AggregateRepository(IEventStore eventStore)
        {
            m_eventStore = eventStore;
        }



        public void Create(TAggregate aggregate)
        {
            var changes = aggregate.GetUncommitedChanges();

            var streamName = StreamNameFor(aggregate);

            m_eventStore.CreateNewStream(streamName, changes);

            aggregate.MarkChangesAsCommited();
        }

        public List<TAggregate> Get()
        {
            return m_aggregates.Values.ToList();
        }

        public TAggregate Get(Guid aggregateId)
        {
            return m_aggregates[aggregateId];
        }

        public void Save(TAggregate aggregate)
        {
            if (aggregate.IsDirty)
            {
                this.SaveAggregateEvents(aggregate);

                m_aggregates[aggregate.Id] = aggregate;
            }
        }




        private void SaveAggregateEvents(TAggregate aggregate)
        {
            var changes = aggregate.GetUncommitedChanges();

            var streamName = StreamNameFor(aggregate);
            
            m_eventStore.AppendEventsToStream(streamName, changes);

            aggregate.MarkChangesAsCommited();
        }

        private string StreamNameFor(TAggregate aggregate)
        {
            // Stream per-aggregate: {AggregateType}-{AggregateId}
            return $"{typeof(TAggregate).Name}-{aggregate.Id}";
        }
    }
}
