using Serilog;

namespace Shared.Core.Logging.Strategies
{
    public class FilePerModuleLoggingStrategy : IModuleLoggingStrategy
    {
        public ILogger CreateLogger(string moduleName)
        {
            // Crear directorio de logs para el módulo
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs", moduleName);
            Directory.CreateDirectory(logDirectory);

            // Configuración de Serilog
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Module", moduleName)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Module}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "log-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{Module}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 15,
                    shared: true
                )
                .CreateLogger();
        }
    }
}