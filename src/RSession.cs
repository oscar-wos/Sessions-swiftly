using Microsoft.Extensions.DependencyInjection;
using RSession.API.Contracts.Core;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Event;
using RSession.API.Contracts.Log;
using RSession.API.Contracts.Schedule;
using RSession.API.Models.Config;
using RSession.Extensions;
using RSession.Services.Core;
using RSession.Services.Log;
using RSession.Services.Schedule;
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

    private IServerService? _serverService;
    private IIntervalService? _intervalService;

    public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddDatabases();
        _ = services.AddEvents();

        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IEventService, EventService>();

        _ = services.AddSingleton<IPlayerService, PlayerService>();
        _ = services.AddSingleton<IServerService, ServerService>();

        _ = services.AddSingleton<IIntervalService, IntervalService>();

        _ = services.AddOptionsWithValidateOnStart<DatabaseConfig>().BindConfiguration("database");
        _ = services.AddOptionsWithValidateOnStart<SessionConfig>().BindConfiguration("config");

        _serviceProvider = services.BuildServiceProvider();

        _serverService = _serviceProvider.GetRequiredService<IServerService>();
        _intervalService = _serviceProvider.GetRequiredService<IIntervalService>();

        interfaceManager.AddSharedInterface<IDatabaseService, IDatabaseService>(
            "RSession.DatabaseService",
            _serviceProvider.GetRequiredService<IDatabaseFactory>().Database
        );

        interfaceManager.AddSharedInterface<IEventService, IEventService>(
            "RSession.EventService",
            _serviceProvider.GetRequiredService<IEventService>()
        );

        interfaceManager.AddSharedInterface<IPlayerService, IPlayerService>(
            "RSession.PlayerService",
            _serviceProvider.GetRequiredService<IPlayerService>()
        );

        interfaceManager.AddSharedInterface<IServerService, IServerService>(
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
