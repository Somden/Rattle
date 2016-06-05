using IdentityServer4.Core.Configuration;
using IdentityServer4.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Rattle.OAuth.Data;
using Rattle.OAuth.Model;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Rattle.OAuth.Configuration
{
    public static class IdentityServerConfiguration
    {
        public static IServiceCollection AddConfiguredIdentityServer(this IServiceCollection serviceCollection, string certificate, string certificatePassword)
        {
            var provider = serviceCollection.BuildServiceProvider();

            var userRepository = provider.GetService<IRepository<User>>();
            var clientRepository = provider.GetService<IRepository<Client>>();
            var scopeRepository = provider.GetService<IRepository<Scope>>();

            var options = new IdentityServerOptions()
            {
                EnableWelcomePage = true,
                SigningCertificate = new X509Certificate2(certificate, certificatePassword),
                RequireSsl = false
            };

            var clients = clientRepository.Get();
            var scopes = scopeRepository.Get();
            var users = userRepository.Get().Select(u => u.ToInMemoryUser()).ToList();

            return serviceCollection.AddIdentityServer(options)
                .AddInMemoryClients(clients)
                .AddInMemoryScopes(scopes)
                .AddInMemoryUsers(users)
                .Services;
        }
    }
}
