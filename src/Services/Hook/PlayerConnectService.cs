using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;

namespace Sessions.Services.Hook;

public sealed class PlayerConnectService : IPlayerConnectService
{
    private readonly ILogService _logService;
    private readonly ILogger<PlayerConnectService> _logger;

    public PlayerConnectService(ILogService logService, ILogger<PlayerConnectService> logger)
    {
        _logService = logService;
        _logger = logger;
    }
}
