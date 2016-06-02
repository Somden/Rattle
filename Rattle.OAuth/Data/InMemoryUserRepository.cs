using System.Collections.Generic;
using Rattle.OAuth.Model;

namespace Rattle.OAuth.Data
{
    public class InMemoryUserRepository : IRepository<User>
    {
        public IEnumerable<User> Get()
        {
            return new[]
            {
                new User("test@test.com", "ab1234")
            };
        }
    }
}
