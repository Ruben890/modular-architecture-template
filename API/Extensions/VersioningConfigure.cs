using Asp.Versioning;

namespace API.Extensions
{
    public static class VersioningConfigure
    {
        public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration config, Action<ApiVersioningOptions>? configureOptions = null)
        {
            services.AddApiVersioning(options =>
            {
                // Configurar el selector de versión actual
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);

                // Asumir la versión predeterminada cuando no se especifique ninguna
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Configurar la versión predeterminada desde la configuración o usar la predeterminada
                var defaultVersion = config["API_Versioning:DEFAULT_VERSION"] ?? "1.0";
                options.DefaultApiVersion = ParseVersion(defaultVersion);

                // Informar las versiones de la API en las respuestas
                options.ReportApiVersions = true;

                // Configurar cómo se lee la versión de la API
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-version"),
                    new QueryStringApiVersionReader("ApiVersion"),
                    new UrlSegmentApiVersionReader());

                // Aplicar configuración adicional si se proporciona
                configureOptions?.Invoke(options);
            }).AddApiExplorer(options =>
            {
                // Formato de nombre del grupo para la documentación
                options.GroupNameFormat = "'v'V";
            });
        }

        private static ApiVersion ParseVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return new ApiVersion(1, 0); // Versión predeterminada si no se proporciona

            var versionParts = version.Split('.');
            if (versionParts.Length == 2 &&
                int.TryParse(versionParts[0], out int major) &&
                int.TryParse(versionParts[1], out int minor))
            {
                return new ApiVersion(major, minor);
            }

            return new ApiVersion(1, 0); // Versión predeterminada si el formato es incorrecto
        }
    }
}
