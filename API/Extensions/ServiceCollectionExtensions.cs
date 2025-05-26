
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace API.Extensions
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

        public static void ConfigureAuthJWT(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 options.IncludeErrorDetails = true;
                 options.RequireHttpsMetadata = true;
                 options.SaveToken = true;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ClockSkew = TimeSpan.Zero,
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = config.GetRequiredSection("Jwt").GetRequiredSection("Issuer").Value,
                     ValidAudience = config.GetRequiredSection("Jwt").GetRequiredSection("Audience").Value,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                 };
                 
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken)
                            && !string.IsNullOrEmpty(accessToken))
                        {
                            ctx.Token = accessToken;
                        }
                
                        await Task.CompletedTask;
                    },
                };
                 
             });
        }

        public static void AddConfiguredControllers(this IServiceCollection services)
        {
            services.AddControllers(config =>
                {
                    config.Filters.Add<StandardResponseFilter>();
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

        public static void AddGlobalCookiePolicy(this IServiceCollection services, IHostEnvironment environment)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

        }

    }
}


