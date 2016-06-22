using System;
using Rattle.Domain.UserAccounts;

namespace Rattle.Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        UserAccount Get(Guid aggregateId);

        void Create(UserAccount userAccount);

        void Save(UserAccount userAccount);
    }
}