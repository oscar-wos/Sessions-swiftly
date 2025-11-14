using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RLogger;
using RSession.Messages.Contracts.Log;
using SwiftlyS2.Shared;

namespace RSession.Messages.Services.Log;

public sealed class LogService(ISwiftlyCore core) : ILogService, IDisposable
{
    private const string PLUGIN_NAME = "RSession.Messages";

    private readonly Logger _logger = new(
        Path.Join(core.GameDirectory, "logs", $"{PLUGIN_NAME}"),
        accuracy: 1
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogDebug(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogDebug(exception, "{message}", message);
        _logger.Debug(message, exception);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogInformation(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogInformation(exception, "{message}", message);
#endif

        _logger.Information(message, exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogWarning(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogWarning(exception, "{message}", message);
#endif

        _logger.Warning(message, exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogError(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogError(exception, "{message}", message);
#endif

        _logger.Error(message, exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception LogCritical(
        string message,
        Exception? exception = null,
        ILogger? logger = null
    )
    {
#if DEBUG
        logger?.LogCritical(exception, "{message}", message);
#endif

        return _logger.Critical(message, exception);
    }

    public void Dispose()
    {
        _logger.Dispose();
        GC.SuppressFinalize(this);
    }
}
