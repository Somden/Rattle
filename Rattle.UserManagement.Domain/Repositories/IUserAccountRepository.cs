using System;
using Rattle.UserManagement.Domain.Entities;

namespace Rattle.UserManagement.Domain.Repositories
{
    public interface IUserAccountRepository
    {
        User Get(Guid aggregateId);

        void Create(User user);

        void Save(User user);
    }
}