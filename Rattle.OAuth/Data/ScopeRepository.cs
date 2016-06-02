using IdentityServer4.Core.Models;
using System.Collections.Generic;

namespace Rattle.OAuth.Data
{
    public class ScopeRepository : IRepository<Scope>
    {
        public IEnumerable<Scope> Get()
        {
            return StandardScopes.All;
        }
    }
}
