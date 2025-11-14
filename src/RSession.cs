using Microsoft.Extensions.DependencyInjection;
using RSession.Contracts.Core;
using RSession.Contracts.Event;
using RSession.Contracts.Schedule;
using RSession.Extensions;
using RSession.Models.Config;
using RSession.Services.Core;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.SteamAPI;
using Tomlyn.Extensions.Configuration;

namespace RSession;

[PluginMetadata(
    Id = "RSession",
    Version = "0.0.0",
    Name = "RSession",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed partial class RSession(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;

    public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
    {
        if (_serviceProvider is null)
        {
            return;
        }

        interfaceManager.AddSharedInterface<IRSessionEvent, EventService>(
            "RSession.Event",
            _serviceProvider.GetRequiredService<EventService>()
        );

        interfaceManager.AddSharedInterface<IRSessionPlayer, PlayerService>(
            "RSession.Player",
            _serviceProvider.GetRequiredService<PlayerService>()
        );

        interfaceManager.AddSharedInterface<IRSessionServer, ServerService>(
            "RSession.Server",
            _serviceProvider.GetRequiredService<ServerService>()
        );
    }

    public override void Load(bool hotReload)
    {
        _ = Core
            .Configuration.InitializeTomlWithModel<DatabaseConfig>("database.toml", "database")
            .Configure(builder =>
                builder.AddTomlFile("database.toml", optional: false, reloadOnChange: true)
            );

        _ = Core
            .Configuration.InitializeTomlWithModel<SessionConfig>("config.toml", "config")
            .Configure(builder =>
                builder.AddTomlFile("config.toml", optional: false, reloadOnChange: true)
            );

        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddConfigs();
        _ = services.AddDatabases();
        _ = services.AddEvents();
        _ = services.AddServices();

        _serviceProvider = services.BuildServiceProvider();

        foreach (IEventListener listener in _serviceProvider.GetServices<IEventListener>() ?? [])
        {
            listener.Subscribe();
        }

        _serviceProvider.GetService<IInterval>()?.Initialize();

        try
        {
            InteropHelp.TestIfAvailableGameServer();
            _serviceProvider.GetService<IRSessionServerInternal>()?.Initialize();
        }
        catch { }
    }

    public override void Unload()
    {
        foreach (IEventListener listener in _serviceProvider?.GetServices<IEventListener>() ?? [])
        {
            listener.Unsubscribe();
        }

        (_serviceProvider as IDisposable)?.Dispose();
    }
}
