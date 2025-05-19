using Newtonsoft.Json.Serialization;
using Shareds.Core;

namespace Modular_Architecture_Template.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            // Leer la lista de orígenes permitidos desde la configuración
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            if (allowedOrigins is null || allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("No allowed origins have been defined in the settings. Make sure to add the 'AllowedOrigins' section in appsettings.");
            }

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                        .AllowAnyHeader() // Permitir todos los encabezados
                        .AllowAnyMethod() // Permitir todos los métodos (GET, POST, PUT, DELETE, etc.)
                        .AllowCredentials() // Habilitar credenciales
                        .WithExposedHeaders("X-Custom-Header") // Exponer solo los encabezados necesarios
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // Cacheo de preflight (OPTIONS)

                });
            });
        }

        public static void AddConfiguredControllers(this IServiceCollection services)
        {
            services.AddControllers(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
        }
    }
}

