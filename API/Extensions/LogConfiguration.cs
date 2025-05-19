using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using Shareds.Core.Interfaces;
using Shareds.Core.Logging;

namespace Modular_Architecture_Template.Extensions
{
    public static class LogConfiguration
    {
        private const string TextOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}";

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddSingleton<ILoggerManager, LoggerManager>(provider =>
                new LoggerManager(provider.GetRequiredService<ILogger<LoggerManager>>()));
        }

        public static void ConfigureLogHost(this IHostBuilder host, IConfiguration configuration)
        {
            host.UseSerilog((context, services, config) =>
            {
                var environment = context.HostingEnvironment;
                var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "Main");
                Directory.CreateDirectory(logPath);

                config.MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.WithProperty("Application", configuration["AppName"])
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        path: Path.Combine(logPath, "application-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        outputTemplate: TextOutputTemplate,
                        rollOnFileSizeLimit: true,
                        shared: true
                    );

                if (environment.IsDevelopment())
                {
                    config.WriteTo.Console(
                        restrictedToMinimumLevel: LogEventLevel.Debug,
                        theme: AnsiConsoleTheme.Code,
                        outputTemplate: TextOutputTemplate
                    );
                }
                else
                {
                    config.WriteTo.Console(new CompactJsonFormatter());
                }
            });
        }
    }
}
