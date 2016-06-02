using IdentityServer4.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rattle.OAuth.Data
{
    public class ClientRepository : IRepository<Client>
    {
        public IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "web",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientName = "Web server",
                    AllowedScopes = StandardScopes.All.Select(s => s.Name).ToList(),
                    Enabled = true
                }
            };
        }
    }    
}
