using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;
using Sessions.Services.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Sessions.Services.Hook;

internal sealed class HookManager : IHookManager, IDisposable
{
    private readonly ISwiftlyCore _core;
    private readonly IServiceProvider _services;

    private readonly ILogService _logService;
    private readonly ILogger<HookManager> _logger;

    private readonly IPlayerAuthorizeService _playerAuthorizeService;
    private readonly IPlayerMessageService _playerMessageService;

    private Guid _cUserMessageSayText2Guid;

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

        _playerAuthorizeService = _services.GetRequiredService<IPlayerAuthorizeService>();
        _playerMessageService = _services.GetRequiredService<IPlayerMessageService>();
    }

    public void Init()
    {
        _core.Event.OnClientSteamAuthorize += _playerAuthorizeService.OnClientSteamAuthorize;

        _logService.LogInformation("CUserMessageSayText2 hooked", logger: _logger);

        _cUserMessageSayText2Guid = _core.NetMessage.HookServerMessage<CUserMessageSayText2>(msg =>
        {
            _playerMessageService.OnClientMessage(in msg);
            return HookResult.Continue;
        });

        if (_cUserMessageSayText2Guid != Guid.Empty)
        {
            _logService.LogInformation(
                $"CUserMessageSayText2 hooked {_cUserMessageSayText2Guid}",
                logger: _logger
            );
        }
        else
        {
            _logService.LogWarning("CUserMessageSayText2 not hooked", logger: _logger);
        }

        _logService.LogInformation("HookManager initialized", logger: _logger);
    }

    public void Dispose()
    {
        _logService.LogInformation("Disposing HookManager", logger: _logger);

        _core.Event.OnClientSteamAuthorize -= _playerAuthorizeService.OnClientSteamAuthorize;
        _core.NetMessage.Unhook(_cUserMessageSayText2Guid);
    }
}
