using Microsoft.Extensions.DependencyInjection;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
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

        interfaceManager.AddSharedInterface<ISessionEventService, EventService>(
            "RSession.EventService",
            _serviceProvider.GetRequiredService<EventService>()
        );

        interfaceManager.AddSharedInterface<ISessionPlayerService, PlayerService>(
            "RSession.PlayerService",
            _serviceProvider.GetRequiredService<PlayerService>()
        );

        interfaceManager.AddSharedInterface<ISessionServerService, ServerService>(
            "RSession.ServerService",
            _serviceProvider.GetRequiredService<ServerService>()
        );
    }

    public override void OnSharedInterfaceInjected(IInterfaceManager interfaceManager) =>
        _serviceProvider?.GetRequiredService<IDatabaseFactory>().InvokeDatabaseConfigured();

    public override void Load(bool hotReload)
    {
        _ = Core
            .Configuration.InitializeTomlWithModel<SessionConfig>("config.toml", "config")
            .Configure(builder =>
                builder.AddTomlFile("config.toml", optional: false, reloadOnChange: true)
            );

        _ = Core
            .Configuration.InitializeTomlWithModel<DatabaseConfig>("database.toml", "database")
            .Configure(builder =>
                builder.AddTomlFile("database.toml", optional: false, reloadOnChange: true)
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

        _serviceProvider.GetService<IIntervalService>()?.Initialize();

        try
        {
            InteropHelp.TestIfAvailableGameServer();
            _serviceProvider.GetService<IServerService>()?.Initialize();
        }
        catch { }
    }

    public override async void Unload()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
    }
}
