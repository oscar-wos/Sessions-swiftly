using Microsoft.Extensions.DependencyInjection;
using RSession.Messages.Contracts.Core;
using RSession.Messages.Contracts.Event;
using RSession.Messages.Contracts.Hook;
using RSession.Messages.Extensions;
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
            ISessionEventService sessionEventService =
                interfaceManager.GetSharedInterface<ISessionEventService>("RSession.EventService");

            _serviceProvider
                ?.GetService<IOnDatabaseConfiguredService>()
                ?.Initialize(sessionEventService);
        }

        if (interfaceManager.HasSharedInterface("RSession.PlayerService"))
        {
            ISessionPlayerService sessionPlayerService =
                interfaceManager.GetSharedInterface<ISessionPlayerService>(
                    "RSession.PlayerService"
                );

            _serviceProvider?.GetService<IPlayerService>()?.Initialize(sessionPlayerService);
        }
    }

    public override void Load(bool hotReload)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddDatabases();
        _ = services.AddEvents();
        _ = services.AddHooks();
        _ = services.AddServices();

        _serviceProvider = services.BuildServiceProvider();

        foreach (IHook hook in _serviceProvider.GetServices<IHook>())
        {
            hook.Register();
        }
    }

    public override void Unload() => (_serviceProvider as IDisposable)?.Dispose();
}
