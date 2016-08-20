using System;
using Rattle.Core.Events;

namespace Rattle.UserManagement.Contracts.Events
{
    public class UsernameChangedEvent : IAggregateEvent
    {
        public UsernameChangedEvent(Guid aggregateId, int version, string username)
        {
            this.AggregateId = aggregateId;
            this.Version = version;
            Username = username;
        }

        public Guid AggregateId { get; }

        public int Version { get; }

        public string Username { get; }
    }
}