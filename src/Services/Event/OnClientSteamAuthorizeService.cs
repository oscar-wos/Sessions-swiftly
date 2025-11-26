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
using RSession.Contracts.Core;
using RSession.Contracts.Event;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;

namespace RSession.Services.Event;

internal sealed class OnClientSteamAuthorizeService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnClientSteamAuthorizeService> logger,
    IPlayerService playerService,
    IServerService serverService
) : IEventListener, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnClientSteamAuthorizeService> _logger = logger;

    private readonly IPlayerService _playerService = playerService;
    private readonly IServerService _serverService = serverService;

    public void Subscribe()
    {
        _core.Event.OnClientSteamAuthorize += OnClientSteamAuthorize;
        _logService.LogInformation("OnClientSteamAuthorize subscribed", logger: _logger);
    }

    private void OnClientSteamAuthorize(IOnClientSteamAuthorizeEvent @event)
    {
        int playerId = @event.PlayerId;

        if (_core.PlayerManager.GetPlayer(playerId) is not { } player)
        {
            _logService.LogWarning(
                $"OnClientSteamAuthorize Player not found - {playerId}",
                logger: _logger
            );

            return;
        }

        if (_serverService.GetServerId() is not { } serverId)
        {
            return;
        }

        _logService.LogDebug(
            $"Authorizing player - {player.Controller.PlayerName} ({player.SteamID})",
            logger: _logger
        );

        _playerService.HandlePlayerAuthorize(player, serverId);
    }

    public void Dispose() => _core.Event.OnClientSteamAuthorize -= OnClientSteamAuthorize;
}
