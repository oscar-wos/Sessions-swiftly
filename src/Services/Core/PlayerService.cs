using Microsoft.Extensions.Logging;
using RSession.API.Contracts.Core;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

public sealed class PlayerService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<PlayerService> logger,
    IDatabaseFactory databaseFactory,
    IEventService eventService
) : IPlayerService
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerService> _logger = logger;

    private readonly IDatabaseService _database = databaseFactory.Database;
    private readonly IEventService _eventService = eventService;

    private readonly Dictionary<ulong, int> _players = [];
    private readonly Dictionary<ulong, long> _sessions = [];

    public int? GetPlayer(IPlayer player) =>
        _players.TryGetValue(player.SteamID, out int playerId) ? playerId : null;

    public long? GetSession(IPlayer player) =>
        _sessions.TryGetValue(player.SteamID, out long sessionId) ? sessionId : null;

    public void Init(short serverId)
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
                    $"Player authorized - {player.Controller.PlayerName} ({player.SteamID}) | Player ID: {playerId} | Session ID: {sessionId}",
                    logger: _logger
                );

                _players[steamId] = playerId;
                _sessions[steamId] = sessionId;

                _eventService.InvokePlayerAuthorized(player, playerId, sessionId);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to authorize player - {player.Controller.PlayerName} ({player.SteamID})",
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
                $"Player not authorized - {player.Controller.PlayerName} ({player.SteamID})",
                logger: _logger
            );

            return;
        }

        _ = _players.Remove(player.SteamID);
        _ = _sessions.Remove(player.SteamID);
    }
}
