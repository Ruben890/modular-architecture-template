using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.User.Application.Services;
using Module.User.Domain.Interfaces.IRepository;
using Module.User.Domain.Interfaces.IServices;
using Module.User.Infrastrutucture;
using Module.User.Presentation.Mappers;
using Shared.Core.DatabaseRetryPolicies.PosgretSQL;
using Shared.Core.Interfaces;
using Shared.Core.Logging;

namespace Module.User
{
    public static class UserModuleStartup
    {
        public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StringConnection")
                                   ?? throw new NullReferenceException("The connection string 'StringConnection' was not found.");

            services.AddDbContext<UserContext>(options =>
            {
                options.UseNpgsql(connectionString);
                var postgresConnectionFactory = new PostgresConnectionFactory(connectionString);
                postgresConnectionFactory.Create();
                postgresConnectionFactory.CreateCommand();
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            TypeAdapterConfig.GlobalSettings.RegisterUserMappings();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserServices, UserServices>();

            services.AddScoped<ILoggerManager>(provider =>
                ModuleLoggerFactory.CreateLoggerManager("User"));
        }
    }
}