using Module.Auth;
using Module.User;

namespace API.Extensions
{
    public static class ConfigurationModules
    {

        public static void RegisterModules(this IServiceCollection services, IConfiguration config)
        {
            services.AddUserModule(config);
            services.AddAuthModule(config);

        }

    }
}
