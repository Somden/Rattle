using System;
using Rattle.Core.Domain;

namespace Rattle.Domain.UserAccounts
{
    public class UserAccount : EventSourcedAggregate
    {
        public int InitialVersion { get; private set; }

        public string UserName { get; private set; }

        public UserAccount()
        {
        }

        public UserAccount(Guid id)
        {
            Causes(new AccountCreated(id));
        }

        public UserAccount(UserAccountSnapshot snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;
            InitialVersion = snapshot.Version;
            UserName = snapshot.UserName;
        }

        public override void Apply(DomainEvent @event)
        {
            When((dynamic)@event);
            Version = Version + 1;
        }

        public UserAccountSnapshot GetUserAccountSnapshot()
        {
            return new UserAccountSnapshot {Version = Version, UserName = UserName, Id = Id};
        }

        public void ChangeUserName(string userName)
        {
            Causes(new UserNameChanged(Id) {UserName = userName});
        }

        public void MarkChangesAsCommited()
        {
            Changes.Clear();
            InitialVersion = Version;
        }

        private void Causes(DomainEvent @event)
        {
            Changes.Add(@event);
            Apply(@event);
        }

        private void When(AccountCreated accountCreated)
        {
            Id = accountCreated.Id;
        }

        private void When(UserNameChanged userNameChanged)
        {
            UserName = userNameChanged.UserName;
        }
    }
}
