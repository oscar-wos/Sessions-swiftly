using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;
using SwiftlyS2.Shared;

namespace Sessions.Services.Hook;

internal sealed class HookManager : IHookManager
{
    private readonly ISwiftlyCore _core;
    private readonly IServiceProvider _services;
    private readonly ILogService _logService;
    private readonly ILogger<HookManager> _logger;

    public HookManager(
        ISwiftlyCore core,
        IServiceProvider services,
        ILogService logService,
        ILogger<HookManager> logger
    )
    {
        _core = core;
        _services = services;
        _logService = logService;
        _logger = logger;
    }
}
