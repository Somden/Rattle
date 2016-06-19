using System;
using Rattle.Domain.UserAccounts;

namespace Rattle.Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        UserAccount FindBy(Guid id);

        void Create(UserAccount userAccount);

        void Save(UserAccount userAccount);
    }
}