﻿using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Core.Interfaces;
using Shared.Core.Logging;

namespace API.Extensions
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
                var logPath = Path.Combine(AppContext.BaseDirectory, "Logs");
                Directory.CreateDirectory(logPath);

                config.MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Wolverine.Runtime", LogEventLevel.Warning)
                    .MinimumLevel.Override("Wolverine.Persistence", LogEventLevel.Warning)
                    .MinimumLevel.Override("Wolverine.Tracking", LogEventLevel.Warning)
                    .Enrich.WithProperty("Application", configuration["AppName"])
                    .Enrich.FromLogContext()
                    .Filter.ByExcluding(logEvent =>
                        logEvent.MessageTemplate.Text.Contains("IAgentCommand") ||
                        logEvent.MessageTemplate.Text.Contains("DatabaseOperationBatch"))
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
