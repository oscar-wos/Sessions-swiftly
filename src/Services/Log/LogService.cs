// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.Logging;
using RLogger;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;
using System.Runtime.CompilerServices;

namespace RSession.Services.Log;

public sealed class LogService(ISwiftlyCore core) : ILogService, IDisposable
{
    private const string PLUGIN_NAME = "RSession";

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
