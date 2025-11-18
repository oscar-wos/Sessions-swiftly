using Microsoft.Extensions.Logging;
using RSession.Maps.Contracts.Core;
using RSession.Maps.Contracts.Database;
using RSession.Maps.Contracts.Log;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared.Players;

namespace RSession.Maps.Services.Core;

internal sealed class PlayerService(
    ILogService logService,
    ILogger<PlayerService> logger,
    IDatabaseFactory databaseFactory
) : IPlayerService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;
    private ISessionPlayerService? _sessionPlayerService;

    public void Initialize(ISessionPlayerService sessionPlayerService) =>
        _sessionPlayerService = sessionPlayerService;

    public void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message) =>
        Task.Run(async () =>
        {
            if (_sessionPlayerService?.GetSessionId(player) is not { } sessionId)
            {
                _logService.LogWarning(
                    $"Player not registered - {player.Controller.PlayerName} ({player.SteamID})",
                    logger: _logger
                );

                return;
            }

            if (_databaseFactory.GetDatabaseService() is { } databaseService)
            {
                try
                {
                    await databaseService.InsertMessageAsync(sessionId, teamNum, teamChat, message);
                }
                catch (Exception ex)
                {
                    _logService.LogError(
                        $"Unable to insert message - {player.Controller.PlayerName} ({player.SteamID}) : {message}",
                        exception: ex,
                        logger: _logger
                    );
                }
            }
        });
}
