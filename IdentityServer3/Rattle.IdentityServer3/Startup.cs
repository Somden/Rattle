using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Rattle.IdentityServer3.Startup))]

namespace Rattle.IdentityServer3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var inMemoryManager = new InMemoryManager();
            var factory =
                new IdentityServerServiceFactory().UseInMemoryUsers(inMemoryManager.GetUsers())
                    .UseInMemoryClients(inMemoryManager.GetClients())
                    .UseInMemoryScopes(inMemoryManager.GetScopes());

            var certificate = Convert.FromBase64String(ConfigurationManager.AppSettings["SigningCertificate"]);
            var options = new IdentityServerOptions()
            {
                SigningCertificate =
                    new X509Certificate2(certificate, ConfigurationManager.AppSettings["SigningCertificatePassword"]),
                RequireSsl = false,
                Factory = factory
            };

            app.UseIdentityServer(options);
        }
    }
}
