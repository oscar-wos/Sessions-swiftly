using System.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RSession.API.Contracts.Core;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Log;
using RSession.API.Contracts.Schedule;
using RSession.API.Models.Config;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using Timer = System.Timers.Timer;

namespace RSession.Services.Schedule;

public sealed class IntervalService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<IntervalService> logger,
    IOptionsMonitor<SessionConfig> config,
    IDatabaseFactory databaseFactory,
    IPlayerService playerService
) : IIntervalService, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<IntervalService> _logger = logger;
    private readonly IOptionsMonitor<SessionConfig> _config = config;

    private readonly IDatabaseService _database = databaseFactory.Database;
    private readonly IPlayerService _playerService = playerService;

    private Timer? _timer;

    public void Init()
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
            if (_playerService.GetPlayer(player) is { } playerId)
            {
                playerIds.Add(playerId);
            }

            if (_playerService.GetSession(player) is { } sessionId)
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
