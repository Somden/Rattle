using Host.UI;
using Host.UI.Login;
using IdentityServer4.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rattle.OAuth.Configuration;
using Rattle.OAuth.Data;
using Rattle.OAuth.Model;
using System;

namespace Rattle.OAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            this.Configuration = env.SetupConfiguration();
        }

        public IConfiguration Configuration { get; } 

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var certificate = Configuration["certificate"];
            var certificatePassword = Configuration["certificatePassword"];

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                    .AddSingleton<IRepository<User>, InMemoryUserRepository>()
                    .AddSingleton<IRepository<Scope>, ScopeRepository>()
                    .AddSingleton<IRepository<Client>, ClientRepository>()
                    .AddConfiguredIdentityServer(certificate, certificatePassword)
                    .AddMvc()
                    .AddRazorOptions(razor =>
                    {
                        razor.ViewLocationExpanders.Add(new CustomViewLocationExpander());
                    });
            services.AddTransient<LoginService>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer()
               .UseStaticFiles()
               .UseMvcWithDefaultRoute();
        }
    }
}
