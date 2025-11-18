// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
    IPlayerService playerService
) : IIntervalService, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<IntervalService> _logger = logger;
    private readonly IOptionsMonitor<SessionConfig> _config = config;

    private readonly IDatabaseService _databaseService = databaseFactory.GetDatabaseService();
    private readonly IPlayerService _playerService = playerService;

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
            if (_playerService.GetSessionPlayer(player) is not { } sessionPlayer)
            {
                continue;
            }

            playerIds.Add(sessionPlayer.Id);
            sessionIds.Add(sessionPlayer.Session);
        }

        if (playerIds.Count == 0)
        {
            return;
        }

        try
        {
            await _databaseService.UpdateSessionsAsync(playerIds, sessionIds);
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
        _logService.LogInformation("IntervalService disposed", logger: _logger);
    }
}
