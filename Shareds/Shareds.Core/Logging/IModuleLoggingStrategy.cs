using ILogger = Serilog.ILogger;

namespace Shareds.Core.Logging
{
    public interface IModuleLoggingStrategy
    {
        ILogger CreateLogger(string moduleName);
    }
}
