using Mpdules.User;

namespace Modular_Architecture_Template.Extencions
{
    public static class ConfigurationModules
    {

        public static void RegisterModules(this IServiceCollection services, IConfiguration config)
        {
            services.AddUserModule(config);

        }

    }
}
