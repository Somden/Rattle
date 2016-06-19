using System;
using Rattle.Core.Domain;

namespace Rattle.Domain.UserAccounts
{
    public class UserNameChanged : DomainEvent
    {
        public UserNameChanged(Guid aggregateId) : base(aggregateId)
        {
        }

        public string UserName { get; set; }
    }
}