using Serilog;

namespace Shareds.Core.Logging.Strategies
{
    public class FilePerModuleLoggingStrategy : IModuleLoggingStrategy
    {
        public ILogger CreateLogger(string moduleName)
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs", moduleName);
            Directory.CreateDirectory(logDirectory);

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "log-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 15,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .Enrich.WithProperty("Module", moduleName)
                .CreateLogger();
        }
    }
}
