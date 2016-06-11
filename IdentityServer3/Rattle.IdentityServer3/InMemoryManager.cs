using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;

namespace Rattle.IdentityServer3
{
    public class InMemoryManager
    {
        public List<InMemoryUser> GetUsers()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "test@test.com",
                    Username = "test@test.com",
                    Password = "ab1234"
                }
            };
        }

        public IEnumerable<Scope> GetScopes()
        {
            return StandardScopes.All;
        }

        public IEnumerable<Client> GetClients()
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
                    Flow = Flows.ResourceOwner,
                    Enabled = true,
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        "read"
                    }
                },
            };
        }
    }
}