using System;
using Rattle.Core.Domain;
using Rattle.Core.Aggregates;

namespace Rattle.Domain.UserAccounts
{
    public class UserAccount : Aggregate
    {
        public int InitialVersion { get; private set; }

        public string UserName { get; private set; }

        public UserAccount()
        {
        }

        public UserAccount(Guid id)
        {
            ApplyEvent(new AccountCreated(id, this.Version + 1));
        }

        public UserAccount(UserAccountSnapshot snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;
            InitialVersion = snapshot.Version;
            UserName = snapshot.UserName;
        }

        public UserAccountSnapshot GetUserAccountSnapshot()
        {
            return new UserAccountSnapshot {Version = Version, UserName = UserName, Id = Id};
        }

        public void ChangeUserName(string userName)
        {
            ApplyEvent(new UserNameChanged(Id, this.Version + 1) {UserName = userName});
        }
        
    }
}
