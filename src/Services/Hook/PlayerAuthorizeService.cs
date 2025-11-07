using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;

namespace Sessions.Services.Hook;

public sealed class PlayerAuthorizeService(
    ISwiftlyCore core,
    IDatabaseFactory databaseFactory,
    ILogService logService,
    ILogger<PlayerAuthorizeService> logger
) : IPlayerAuthorizeService
{
    private readonly ISwiftlyCore _core = core;
    private readonly IDatabaseService _database = databaseFactory.Database;

    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerAuthorizeService> _logger = logger;

    public void OnClientSteamAuthorize(IOnClientSteamAuthorizeEvent @event)
    {
        if (_core.PlayerManager.GetPlayer(@event.PlayerId) is not { } player)
        {
            _logService.LogWarning($"Player not authorized - {@event.PlayerId}", logger: _logger);
            return;
        }
    }
}
