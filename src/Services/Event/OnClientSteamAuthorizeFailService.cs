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
using RSession.Contracts.Event;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace RSession.Services.Event;

internal sealed class OnClientSteamAuthorizeFailService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnClientSteamAuthorizeFailService> logger
) : IEventListener
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnClientSteamAuthorizeFailService> _logger = logger;

    public void Subscribe()
    {
        _core.Event.OnClientSteamAuthorizeFail += OnClientSteamAuthorizeFail;
        _logService.LogInformation("OnClientSteamAuthorizeFail subscribed", logger: _logger);
    }

    private void OnClientSteamAuthorizeFail(IOnClientSteamAuthorizeFailEvent @event)
    {
        int playerId = @event.PlayerId;

        if (_core.PlayerManager.GetPlayer(playerId) is not { } player)
        {
            _logService.LogWarning(
                $"OnClientSteamAuthorizeFail Player not found - {playerId}",
                logger: _logger
            );

            return;
        }

        _logService.LogInformation(
            $"Unable to authorize player - {player.Controller.PlayerName} ({player.SteamID})",
            logger: _logger
        );

        player.Kick("No Auth", ENetworkDisconnectionReason.NETWORK_DISCONNECT_STEAM_AUTHINVALID);
    }

    public void Dispose() => _core.Event.OnClientSteamAuthorizeFail -= OnClientSteamAuthorizeFail;
}
