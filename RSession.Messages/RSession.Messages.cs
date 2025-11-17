using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSession.Messages.Contracts.Hook;
using RSession.Messages.Extensions;
using RSession.Messages.Services.Core;
using RSession.Messages.Services.Event;
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
        if (interfaceManager.HasSharedInterface("RSession.EventService"))
        {
            ISessionEventService eventService =
                interfaceManager.GetSharedInterface<ISessionEventService>("RSession.EventService");

            _serviceProvider?.GetService<OnDatabaseConfiguredService>()?.Initialize(eventService);
        }

        if (
            interfaceManager.HasSharedInterface("RSession.PlayerService")
            && interfaceManager.HasSharedInterface("RSession.ServerService")
        )
        {
            ISessionPlayerService playerService =
                interfaceManager.GetSharedInterface<ISessionPlayerService>(
                    "RSession.PlayerService"
                );

            ISessionServerService serverService =
                interfaceManager.GetSharedInterface<ISessionServerService>(
                    "RSession.ServerService"
                );

            _serviceProvider?.GetService<PlayerService>()?.Initialize(playerService, serverService);
        }
    }

    public override void Load(bool hotReload)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddDatabase();
        _ = services.AddServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    public override void Unload()
    {
        foreach (IHook hook in _serviceProvider?.GetServices<IHook>() ?? [])
        {
            hook.Unregister();
        }

        Core.Logger.LogInformation("RSession.Messages Unloaded - disposing");
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
