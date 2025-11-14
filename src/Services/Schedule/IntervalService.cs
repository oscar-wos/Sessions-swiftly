using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using RSession.Contracts.Schedule;
using RSession.Models.Config;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using Timer = System.Timers.Timer;

namespace RSession.Services.Schedule;

internal sealed class IntervalService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<IntervalService> logger,
    IOptionsMonitor<SessionConfig> config,
    IDatabaseFactory databaseFactory,
    IRSessionPlayerInternal playerService
) : IInterval, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<IntervalService> _logger = logger;
    private readonly IOptionsMonitor<SessionConfig> _config = config;

    private readonly IDatabaseService _database = databaseFactory.Database;
    private readonly IRSessionPlayerInternal _playerService = playerService;

    private Timer? _timer;

    public void Initialize()
    {
        _timer = new Timer(_config.CurrentValue.UpdateInterval)
        {
            AutoReset = true,
            Enabled = true,
        };

        _timer.Elapsed += async (_, _) => await OnElapsed();
        _logService.LogInformation("IntervalService initialized", logger: _logger);
    }

    private async Task OnElapsed()
    {
        List<int> playerIds = [];
        List<long> sessionIds = [];

        foreach (IPlayer player in _core.PlayerManager.GetAllPlayers())
        {
            if (_playerService.GetPlayerId(player) is { } playerId)
            {
                playerIds.Add(playerId);
            }

            if (_playerService.GetSessionId(player) is { } sessionId)
            {
                sessionIds.Add(sessionId);
            }
        }

        try
        {
            await _database.UpdateSessionsAsync(playerIds, sessionIds);
        }
        catch (Exception ex)
        {
            _logService.LogError($"IntervalService.OnElapsed()", exception: ex, logger: _logger);
        }
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
