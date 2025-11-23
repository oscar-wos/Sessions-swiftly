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
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using RSession.Shared.Structs;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

internal sealed class PlayerService : IPlayerService, IDisposable
{
    private readonly ISwiftlyCore _core;
    private readonly ILogService _logService;
    private readonly ILogger<PlayerService> _logger;

    private readonly IDatabaseService _databaseService;
    private readonly IEventService _eventService;

    private readonly Dictionary<ulong, SessionPlayer> _players = [];

    public PlayerService(
        ISwiftlyCore core,
        ILogService logService,
        ILogger<PlayerService> logger,
        IDatabaseFactory databaseFactory,
        IEventService eventService
    )
    {
        _core = core;
        _logService = logService;
        _logger = logger;

        _databaseService = databaseFactory.GetDatabaseService();
        _eventService = eventService;

        _eventService.OnElapsed += OnElapsed;
        _eventService.OnServerRegistered += OnServerRegistered;
    }

    public int? GetPlayerId(IPlayer player) =>
        _players.TryGetValue(player.SteamID, out SessionPlayer sessionPlayer)
            ? sessionPlayer.Id
            : null;

    public long? GetSessionId(IPlayer player) =>
        _players.TryGetValue(player.SteamID, out SessionPlayer sessionPlayer)
            ? sessionPlayer.Session
            : null;

    public SessionPlayer? GetSessionPlayer(IPlayer player) =>
        _players.TryGetValue(player.SteamID, out SessionPlayer sessionPlayer)
            ? sessionPlayer
            : null;

    public void HandlePlayerAuthorize(IPlayer player, short serverId) =>
        Task.Run(async () =>
        {
            ulong steamId = player.SteamID;

            try
            {
                int playerId = await _databaseService.GetPlayerAsync(steamId).ConfigureAwait(false);

                long sessionId = await _databaseService
                    .GetSessionAsync(playerId, serverId, player.IPAddress)
                    .ConfigureAwait(false);

                _logService.LogInformation(
                    $"Player registered - {player.Controller.PlayerName} ({player.SteamID}) | Player ID: {playerId} | Session ID: {sessionId}",
                    logger: _logger
                );

                SessionPlayer sessionPlayer = new() { Id = playerId, Session = sessionId };
                _players[steamId] = sessionPlayer;

                _eventService.InvokePlayerRegistered(player, in sessionPlayer);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to register player - {player.Controller.PlayerName} ({player.SteamID})",
                    exception: ex,
                    logger: _logger
                );
            }
        });

    public void HandlePlayerDisconnected(IPlayer player)
    {
        if (GetSessionPlayer(player) is null)
        {
            _logService.LogWarning(
                $"Player not registered - {player.Controller.PlayerName} ({player.SteamID})",
                logger: _logger
            );

            return;
        }

        _ = _players.Remove(player.SteamID);
    }

    private void OnElapsed(int interval) =>
        Task.Run(async () =>
        {
            List<int> playerIds = [];
            List<long> sessionIds = [];

            foreach (IPlayer player in _core.PlayerManager.GetAllPlayers())
            {
                if (GetSessionPlayer(player) is not { } sessionPlayer)
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
                _logService.LogError($"PlayerService.OnElapsed()", exception: ex, logger: _logger);
            }
        });

    private void OnServerRegistered(short serverId)
    {
        foreach (IPlayer player in _core.PlayerManager.GetAllPlayers())
        {
            if (player is not { IsAuthorized: true })
            {
                continue;
            }

            HandlePlayerAuthorize(player, serverId);
        }
    }

    public void Dispose()
    {
        _eventService.OnElapsed -= OnElapsed;
        _eventService.OnServerRegistered -= OnServerRegistered;
    }
}
