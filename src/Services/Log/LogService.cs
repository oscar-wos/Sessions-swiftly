using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RLogger;
using Sessions.API.Contracts.Log;
using SwiftlyS2.Shared;

namespace Sessions.Services.Log;

public class LogService(ISwiftlyCore core) : ILogService, IDisposable
{
    private const string PLUGIN_NAME = "Sessions";

    private readonly Logger _logger = new(
        Path.Join(core.GameDirectory, "logs", $"{PLUGIN_NAME}"),
        accuracy: 1
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogDebug(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogDebug(exception, "{message}", message);
#endif

        _logger.Debug($"[{PLUGIN_NAME}] {message}", exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogInformation(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogInformation(exception, "{message}", message);
#endif

        _logger.Information($"[{PLUGIN_NAME}] {message}", exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogWarning(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogWarning(exception, "{message}", message);
#endif

        _logger.Warning($"[{PLUGIN_NAME}] {message}", exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogError(string message, Exception? exception = null, ILogger? logger = null)
    {
#if DEBUG
        logger?.LogError(exception, "{message}", message);
#endif

        _logger.Error($"[{PLUGIN_NAME}] {message}", exception);
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

        return _logger.Critical($"[{PLUGIN_NAME}] {message}", exception);
    }

    public void Dispose()
    {
        _logger.Dispose();
        GC.SuppressFinalize(this);
    }
}
