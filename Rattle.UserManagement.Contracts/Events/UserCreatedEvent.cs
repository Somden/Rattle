using System;
using Rattle.Core.Events;

namespace Rattle.UserManagement.Contracts.Events
{
    public class UserCreatedEvent : IAggregateEvent
    {
        public UserCreatedEvent(Guid aggregateId, int version, string username, string password)
        {
            AggregateId = aggregateId;
            Version = version;

            Username = username;
            Password = password;
        }

        public Guid AggregateId { get; }

        public int Version { get; }

        public string Username { get; }

        public string Password { get; }
    }
}