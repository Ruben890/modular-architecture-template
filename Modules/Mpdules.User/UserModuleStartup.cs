using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mpdules.User.Infrastrutucture;
using Mpdules.User.Presentation.Mappers;
using Shareds.Core.DatabaseRetryPolicies.PosgretSQL;
using Shareds.Core.Interfaces;
using Shareds.Core.Logging;

namespace Mpdules.User
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

            // Configuración de Mapster específica del módulo User
            TypeAdapterConfig.GlobalSettings.RegisterUserMappings();

            services.AddScoped<ILoggerManager>(provider =>
                ModuleLoggerFactory.CreateLoggerManager("User"));
        }
    }
}
