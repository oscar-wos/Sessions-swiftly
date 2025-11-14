using Microsoft.Extensions.DependencyInjection;
using RSession.Messages.Contracts.Core;
using RSession.Messages.Contracts.Hook;
using RSession.Messages.Contracts.Log;
using RSession.Messages.Services.Core;
using RSession.Messages.Services.Log;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace RSession.Messages;

[PluginMetadata(
    Id = "RSession.Messages",
    Version = "0.0.0",
    Name = "RSession.Messages",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed partial class Messages(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;

    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        if (
            !interfaceManager.HasSharedInterface("RSession.Player")
            || !interfaceManager.HasSharedInterface("RSession.Server")
        )
        {
            return;
        }

        IRSessionPlayer sessionPlayer = interfaceManager.GetSharedInterface<IRSessionPlayer>(
            "RSession.Player"
        );

        IRSessionServer sessionServer = interfaceManager.GetSharedInterface<IRSessionServer>(
            "RSession.Server"
        );

        _serviceProvider
            ?.GetRequiredService<IPlayerService>()
            .Initialize(sessionPlayer, sessionServer);
    }

    public override void Load(bool hotReload)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IPlayerService, PlayerService>();

        _ = services.AddSingleton<IHook, OnUserMessageSayText2Service>();

        _serviceProvider = services.BuildServiceProvider();

        foreach (IHook hook in _serviceProvider.GetServices<IHook>())
        {
            hook.Register();
        }
    }

    public override void Unload()
    {
        foreach (IHook hook in _serviceProvider?.GetServices<IHook>() ?? [])
        {
            hook.Unregister();
        }

        (_serviceProvider as IDisposable)?.Dispose();
    }
}
