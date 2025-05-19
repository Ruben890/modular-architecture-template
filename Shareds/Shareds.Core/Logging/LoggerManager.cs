using Microsoft.Extensions.Logging;
using Shareds.Core.Interfaces;

namespace Shareds.Core.Logging
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger<LoggerManager> _logger;

        // Constructor to initialize the logger
        public LoggerManager(ILogger<LoggerManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogDebug(string message, object? data = null)
        {
            message = TruncateLongString(message, 500);
            _logger.LogDebug("Message: {Message} | Data: {@Data}", message, data);
        }

        public void LogError(string message, object? data = null)
        {
            message = TruncateLongString(message, 500);
            _logger.LogError("Message: {Message} | Data: {@Data}", message, data);
        }

        public void LogInfo(string message, object? data = null)
        {
            message = TruncateLongString(message, 500);
            _logger.LogInformation("Message: {Message} | Data: {@Data}", message, data);
        }

        public void LogWarn(string message, object? data = null)
        {
            message = TruncateLongString(message, 500);
            _logger.LogWarning("Message: {Message} | Data: {@Data}", message, data);
        }

        private string TruncateLongString(string message, int maxLength)
        {
            if (string.IsNullOrEmpty(message)) return message;
            return message.Length > maxLength ? message.Substring(0, maxLength) : message;
        }
    }
}