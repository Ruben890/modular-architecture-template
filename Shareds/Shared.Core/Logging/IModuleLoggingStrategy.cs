using ILogger = Serilog.ILogger;

namespace Shared.Core.Logging
{
    public interface IModuleLoggingStrategy
    {
        ILogger CreateLogger(string moduleName);
    }
}
