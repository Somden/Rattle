﻿using System;
using Rattle.Core.Domain;
using Rattle.Domain.UserAccounts;

namespace Rattle.Infrastructure.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IEventStore _eventStore;

        public UserAccountRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public UserAccount FindBy(Guid id)
        {
            var streamName = StreamNameFor(id);

            var fromEventNumber = 0;
            var toEventNumber = int.MaxValue;
            var stream = _eventStore.GetStream(streamName, fromEventNumber, toEventNumber);

            UserAccount userAccount = new UserAccount();

            foreach (var @event in stream)
            {
                userAccount.Apply(@event);
            }

            return userAccount;
        }

        public void Create(UserAccount userAccount)
        {
            var streamName = StreamNameFor(userAccount.Id);

            _eventStore.CreateNewStream(streamName, userAccount.Changes);
        }

        public void Save(UserAccount userAccount)
        {
            var streamName = StreamNameFor(userAccount.Id);

            var expectedVersion = GetExpectedVersion(userAccount.InitialVersion);
            _eventStore.AppendEventsToStream(streamName, userAccount.Changes, expectedVersion);
        }

        private int? GetExpectedVersion(int expectedVersion)
        {
            if (expectedVersion == 0)
            {
                // first time the aggregate is stored there is no expected version
                return null;
            }
            else
            {
                return expectedVersion;
            }
        }

        private string StreamNameFor(Guid id)
        {
            // Stream per-aggregate: {AggregateType}-{AggregateId}
            return $"{typeof(UserAccount).Name}-{id}";
        }
    }
}
