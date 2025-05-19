using Microsoft.Extensions.Logging;
using Serilog;
using Shareds.Core.Interfaces;
using Shareds.Core.Logging.Strategies;

namespace Shareds.Core.Logging
{
    public static class ModuleLoggerFactory
    {
        private static readonly IModuleLoggingStrategy _strategy = new FilePerModuleLoggingStrategy();

        public static ILoggerManager CreateLoggerManager(string moduleName)
        {
            // Configura Serilog con la estrategia por módulo
            var serilogLogger = _strategy.CreateLogger(moduleName);

            // Crea el LoggerFactory de Microsoft.Extensions.Logging
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(serilogLogger, dispose: true); // Libera recursos al finalizar
            });

            // Retorna una instancia de LoggerManager con el logger tipado
            return new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());
        }
    }
}

