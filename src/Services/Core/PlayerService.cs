using Microsoft.Extensions.Logging;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Shared.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

internal sealed class PlayerService : IRSessionPlayerInternal, IDisposable
{
    private readonly ISwiftlyCore _core;
    private readonly IRSessionLog _logService;
    private readonly ILogger<PlayerService> _logger;

    private readonly IDatabaseService _database;
    private readonly IRSessionEventInternal _eventService;

    private readonly Dictionary<ulong, int> _players = [];
    private readonly Dictionary<ulong, long> _sessions = [];

    public PlayerService(
        ISwiftlyCore core,
        IRSessionLog logService,
        ILogger<PlayerService> logger,
        IDatabaseFactory databaseFactory,
        IRSessionEventInternal eventService
    )
    {
        _core = core;
        _logService = logService;
        _logger = logger;

        _database = databaseFactory.Database;
        _eventService = eventService;

        _eventService.OnServerRegistered += OnServerRegistered;
    }

    public int? GetPlayer(IPlayer player) =>
        _players.TryGetValue(player.SteamID, out int playerId) ? playerId : null;

    public long? GetSession(IPlayer player) =>
        _sessions.TryGetValue(player.SteamID, out long sessionId) ? sessionId : null;

    public void HandlePlayerAuthorize(IPlayer player, short serverId) =>
        Task.Run(async () =>
        {
            ulong steamId = player.SteamID;

            try
            {
                int playerId = await _database.GetPlayerAsync(steamId).ConfigureAwait(false);

                long sessionId = await _database
                    .GetSessionAsync(playerId, serverId, player.IPAddress)
                    .ConfigureAwait(false);

                _logService.LogInformation(
                    $"Player registered - {player.Controller.PlayerName} ({player.SteamID}) | Player ID: {playerId} | Session ID: {sessionId}",
                    logger: _logger
                );

                _players[steamId] = playerId;
                _sessions[steamId] = sessionId;

                _eventService.InvokePlayerRegistered(player, playerId, sessionId);
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
        if (GetPlayer(player) is null)
        {
            _logService.LogWarning(
                $"Player not registered - {player.Controller.PlayerName} ({player.SteamID})",
                logger: _logger
            );

            return;
        }

        _ = _players.Remove(player.SteamID);
        _ = _sessions.Remove(player.SteamID);
    }

    private void OnServerRegistered(short serverId)
    {
        foreach (IPlayer player in _core.PlayerManager.GetAllPlayers())
        {
            if (player is not { IsValid: true, IsAuthorized: true })
            {
                continue;
            }

            HandlePlayerAuthorize(player, serverId);
        }
    }

    public void Dispose() => _eventService.OnServerRegistered -= OnServerRegistered;
}
