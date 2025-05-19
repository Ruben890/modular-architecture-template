namespace Shareds.Core.Interfaces
{
    public interface ILoggerManager
    {
        void LogDebug(string message, object? data = null);
        void LogError(string message, object? data = null);
        void LogInfo(string message, object? data = null);
        void LogWarn(string message, object? data = null);
    }
}
