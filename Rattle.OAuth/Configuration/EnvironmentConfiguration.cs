using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Rattle.OAuth.Configuration
{
    public static class EnvironmentConfiguration
    {
        public static IConfigurationRoot SetupConfiguration(this IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            return builder.Build();
        } 
    }
}
