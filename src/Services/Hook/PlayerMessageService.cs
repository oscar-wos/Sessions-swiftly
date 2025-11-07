using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;

namespace Sessions.Services.Hook;

public sealed class PlayerMessageService : IPlayerMessageService
{
    private readonly ILogService _logService;
    private readonly ILogger<PlayerMessageService> _logger;

    public PlayerMessageService(ILogService logService, ILogger<PlayerMessageService> logger)
    {
        _logService = logService;
        _logger = logger;
    }
}
