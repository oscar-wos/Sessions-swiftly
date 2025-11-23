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
using RSession.Played.Contracts.Core;
using RSession.Played.Contracts.Database;
using RSession.Played.Contracts.Log;
using RSession.Shared.Contracts.Core;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace RSession.Played.Services.Core;

internal sealed class PlayerService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<PlayerService> logger,
    IDatabaseFactory databaseFactory
) : IPlayerService
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;
    private ISessionPlayerService? _sessionPlayerService;

    public void Initialize(ISessionPlayerService sessionPlayerService) =>
        _sessionPlayerService = sessionPlayerService;

    public void HandleElapsed(int interval) =>
        Task.Run(async () =>
        {
            if (_databaseFactory.GetDatabaseService() is not { } databaseService)
            {
                _logService.LogWarning("Database service not available", logger: _logger);
                return;
            }

            if (_sessionPlayerService is null)
            {
                _logService.LogWarning("Session player service not available", logger: _logger);
                return;
            }

            List<long> aliveT = [];
            List<long> aliveCT = [];
            List<long> deadT = [];
            List<long> deadCT = [];
            List<long> spec = [];

            foreach (IPlayer player in _core.PlayerManager.GetAllPlayers())
            {
                if (_sessionPlayerService.GetSessionId(player) is not { } sessionId)
                {
                    continue;
                }

                bool isAlive = player.PlayerPawn?.LifeState == (byte)LifeState_t.LIFE_ALIVE;

                switch (player.Controller.Team)
                {
                    case Team.T when isAlive:
                        aliveT.Add(sessionId);
                        break;

                    case Team.CT when isAlive:
                        aliveCT.Add(sessionId);
                        break;

                    case Team.T:
                        deadT.Add(sessionId);
                        break;

                    case Team.CT:
                        deadCT.Add(sessionId);
                        break;

                    default:
                        spec.Add(sessionId);
                        break;
                }
            }

            try
            {
                await databaseService
                    .UpdatePlayedAsync(
                        [.. aliveT],
                        [.. aliveCT],
                        [.. deadT],
                        [.. deadCT],
                        [.. spec],
                        interval
                    )
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.LogError($"Unable to update played", exception: ex, logger: _logger);
            }
        });

    public void HandlePlayerRegistered(IPlayer player, long sessionId) =>
        Task.Run(async () =>
        {
            if (_databaseFactory.GetDatabaseService() is not { } databaseService)
            {
                _logService.LogWarning("Database service not available", logger: _logger);
                return;
            }

            try
            {
                await databaseService.InsertPlayedAsync(sessionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to insert played - {player.Controller.PlayerName} ({player.SteamID})",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
