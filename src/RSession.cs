using Microsoft.Extensions.DependencyInjection;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Event;
using RSession.Contracts.Schedule;
using RSession.Extensions;
using RSession.Models.Config;
using RSession.Services.Core;
using RSession.Services.Log;
using RSession.Services.Schedule;
using RSession.Shared.Contracts.Core;
using RSession.Shared.Contracts.Log;
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

    private IRSessionServerInternal? _serverService;
    private IInterval? _intervalService;

    public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddDatabases();
        _ = services.AddEvents();

        _ = services.AddSingleton<IRSessionLog, LogService>();

        _ = services.AddSingleton<IRSessionEventInternal, EventService>();
        _ = services.AddSingleton<IRSessionPlayerInternal, PlayerService>();
        _ = services.AddSingleton<IRSessionServerInternal, ServerService>();

        _ = services.AddSingleton<IInterval, IntervalService>();

        _ = services.AddOptionsWithValidateOnStart<DatabaseConfig>().BindConfiguration("database");
        _ = services.AddOptionsWithValidateOnStart<SessionConfig>().BindConfiguration("config");

        _serviceProvider = services.BuildServiceProvider();

        _serverService = _serviceProvider.GetRequiredService<IRSessionServerInternal>();
        _intervalService = _serviceProvider.GetRequiredService<IInterval>();

        interfaceManager.AddSharedInterface<IDatabaseService, IDatabaseService>(
            "RSession.DatabaseService",
            _serviceProvider.GetRequiredService<IDatabaseFactory>().Database
        );

        interfaceManager.AddSharedInterface<IRSessionEvent, IRSessionEvent>(
            "RSession.EventService",
            _serviceProvider.GetRequiredService<IRSessionEvent>()
        );

        interfaceManager.AddSharedInterface<IRSessionPlayer, IRSessionPlayer>(
            "RSession.PlayerService",
            _serviceProvider.GetRequiredService<IRSessionPlayer>()
        );

        interfaceManager.AddSharedInterface<IRSessionServer, IRSessionServer>(
            "RSession.ServerService",
            _serverService
        );
    }

    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        foreach (IEventListener listener in _serviceProvider?.GetServices<IEventListener>() ?? [])
        {
            listener.Subscribe();
        }

        _intervalService?.Init();

        try
        {
            InteropHelp.TestIfAvailableGameServer();
            _serverService?.Init();
        }
        catch { }
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
