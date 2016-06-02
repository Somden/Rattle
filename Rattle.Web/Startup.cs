using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rattle.Web.Configuration;

namespace Rattle.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            
            Configuration = env.SetupConfiguration();
        }


        public IConfigurationRoot Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }


        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage()
               .UseJwtBearerAuthentication(new JwtBearerOptions
               {
                   Authority = "http://localhost:12105/",
                   RequireHttpsMetadata = false
               })
               .UseMvc();
        }
    }
}
