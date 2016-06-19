using System;
using Rattle.Core.Domain;

namespace Rattle.Domain.UserAccounts
{
    public class AccountCreated : DomainEvent
    {
        public AccountCreated(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}