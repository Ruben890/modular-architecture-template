using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mpdules.User.Infrastrutucture;

namespace Mpdules.User
{
    public static class UserModuleStartup
    {
        public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StringConnection");

            services.AddDbContext<UserContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("connectionString"));
            });


        }
    }

}
