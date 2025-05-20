using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Interfaces;
using Shared.Core.Logging;


namespace Module.Auth
{
    public static class AuthModuleStartup
    {
        public static void AddAuthModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILoggerManager>(provider =>
                ModuleLoggerFactory.CreateLoggerManager("Auth"));
        }
    }
}