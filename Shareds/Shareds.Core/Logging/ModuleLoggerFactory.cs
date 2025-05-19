using Serilog;
using Shareds.Core.Logging.Strategies;

namespace Shareds.Core.Logging
{
    public static class ModuleLoggerFactory
    {
        private static IModuleLoggingStrategy _strategy = new FilePerModuleLoggingStrategy();

        public static ILogger GetLogger(string moduleName)
        {
            return _strategy.CreateLogger(moduleName);
        }
    }
}
