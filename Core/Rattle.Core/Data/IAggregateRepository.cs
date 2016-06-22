using Rattle.Core.Aggregates;
using System;
using System.Collections.Generic;

namespace Rattle.Core.Data
{
    public interface IAggregateRepository<TAggregate> where TAggregate : Aggregate
    {
        void Create(TAggregate aggregate);

        List<TAggregate> Get();

        TAggregate Get(Guid aggregateId);

        void Save(TAggregate aggregate);
    }
}
