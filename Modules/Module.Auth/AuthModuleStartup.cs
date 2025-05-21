using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Auth.Application.Services;
using Module.Auth.Domain.Interfaces;

namespace Module.Auth
{
    public static class AuthModuleStartup
    {
        public static void AddAuthModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IAuthService, AuthService>();
        }
    }
}