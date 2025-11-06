using Microsoft.Extensions.Logging;

namespace Sessions.API.Contracts.Log;

public interface ILogService
{
    void LogDebug(string message, Exception? exception = null, ILogger? logger = null);

    void LogInformation(string message, Exception? exception = null, ILogger? logger = null);

    void LogWarning(string message, Exception? exception = null, ILogger? logger = null);

    void LogError(string message, Exception? exception = null, ILogger? logger = null);

    Exception LogCritical(string message, Exception? exception = null, ILogger? logger = null);
}
