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
    IRSessionPlayerInternal playerService,
    IRSessionServerInternal serverService
) : IEventListener
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnClientSteamAuthorizeService> _logger = logger;

    private readonly IRSessionPlayerInternal _playerService = playerService;
    private readonly IRSessionServerInternal _serverService = serverService;

    public void Subscribe()
    {
        _core.Event.OnClientSteamAuthorize += OnClientSteamAuthorize;
        _logService.LogInformation("OnClientSteamAuthorize subscribed", logger: _logger);
    }

    public void Unsubscribe()
    {
        _core.Event.OnClientSteamAuthorize -= OnClientSteamAuthorize;
        _logService.LogInformation("OnClientSteamAuthorize unsubscribed", logger: _logger);
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
}
