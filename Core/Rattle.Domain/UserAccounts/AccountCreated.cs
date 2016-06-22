using System;
using Rattle.Core.Domain;
using Rattle.Core.Events;

namespace Rattle.Domain.UserAccounts
{
    public class AccountCreated : IAggregateEvent
    {
        public AccountCreated(Guid aggregateId, int version)
        {
            this.AggregateId = aggregateId;
            this.Version = version;
        }

        public Guid AggregateId { get; }

        public int Version { get; }
    }
}