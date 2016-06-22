using System;
using Rattle.Core.Domain;
using Rattle.Domain.UserAccounts;
using Rattle.Core.Data;

namespace Rattle.Infrastructure.Repositories
{
    public class UserAccountRepository : AggregateRepository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(IEventStore eventStore)
            :base(eventStore)
        {
        }

        //public UserAccount Get(Guid id)
        //{
        //    var streamName = StreamNameFor(id);

        //    var fromEventNumber = 0;
        //    var toEventNumber = int.MaxValue;

        //    var snapshot = _eventStore.GetLatestSnapshot<UserAccountSnapshot>(streamName);
        //    if (snapshot != null)
        //    {
        //        fromEventNumber = snapshot.Version + 1; // load only events after snapshot
        //    }

        //    var stream = _eventStore.GetStream(streamName, fromEventNumber, toEventNumber);

        //    UserAccount userAccount = null;
        //    if (snapshot != null)
        //    {
        //        userAccount = new UserAccount(snapshot);
        //    }
        //    else
        //    {
        //        userAccount = new UserAccount();
        //    }

        //    if (stream != null)
        //    {
        //        foreach (var @event in stream)
        //        {
        //            userAccount.Apply(@event);
        //        }
        //    }
        //    userAccount.MarkChangesAsCommited();
        //    return userAccount;
        //}
    }
}
