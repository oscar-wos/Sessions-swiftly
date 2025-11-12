using Microsoft.Extensions.Logging;
using RSession.API.Contracts.Core;
using RSession.API.Contracts.Event;
using RSession.API.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;

namespace RSession.Services.Event;

public sealed class OnClientDisconnectedService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnClientDisconnectedService> logger,
    IPlayerService playerService
) : IEventListener
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnClientDisconnectedService> _logger = logger;

    private readonly IPlayerService _playerService = playerService;

    public void Subscribe()
    {
        _core.Event.OnClientDisconnected += OnClientDisconnected;
        _logService.LogInformation("OnClientDisconnected subscribed", logger: _logger);
    }

    public void Unsubscribe()
    {
        _core.Event.OnClientDisconnected -= OnClientDisconnected;
        _logService.LogInformation("OnClientDisconnected unsubscribed", logger: _logger);
    }

    private void OnClientDisconnected(IOnClientDisconnectedEvent @event)
    {
        int playerId = @event.PlayerId;

        if (_core.PlayerManager.GetPlayer(playerId) is not { } player)
        {
            _logService.LogWarning(
                $"OnClientDisconnected Player not found - {playerId}",
                logger: _logger
            );

            return;
        }

        _logService.LogDebug(
            $"Disconnected player - {player.Controller.PlayerName} ({player.SteamID})",
            logger: _logger
        );

        _playerService.HandlePlayerDisconnected(player);
    }
}
