﻿using System;
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
                m_aggregates[aggregate.Id] = aggregate;
            }
        }
    }
}
