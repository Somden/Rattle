using System;
using System.Collections.Generic;
using Rattle.Core.Aggregates;
using System.Linq;

namespace Rattle.Core.Data
{
    public class AggregateRepository<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : Aggregate
    {
        private readonly Dictionary<Guid, TAggregate> m_aggregates = new Dictionary<Guid, TAggregate>();



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
                //TODO: Save with EventStore
                m_aggregates[aggregate.Id] = aggregate;
            }
        }
    }
}
