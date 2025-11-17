using Microsoft.Extensions.Logging;
using RSession.Contracts.Core;
using RSession.Contracts.Event;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace RSession.Services.Event;

internal sealed class OnClientDisconnectedService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnClientDisconnectedService> logger,
    IPlayerService playerService
) : IEventListener, IDisposable
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

    private void OnClientDisconnected(IOnClientDisconnectedEvent @event)
    {
        if (@event.Reason == ENetworkDisconnectionReason.NETWORK_DISCONNECT_SHUTDOWN)
        {
            return;
        }

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

    public void Dispose()
    {
        _core.Event.OnClientDisconnected -= OnClientDisconnected;
        _logService.LogInformation("OnClientDisconnected disposed", logger: _logger);
    }
}
