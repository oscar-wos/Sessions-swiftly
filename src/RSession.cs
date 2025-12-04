// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.DependencyInjection;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Event;
using RSession.Contracts.Schedule;
using RSession.Extensions;
using RSession.Models.Config;
using RSession.Services.Core;
using RSession.Shared.Contracts.Core;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.SteamAPI;
using Tomlyn.Extensions.Configuration;

namespace RSession;

[PluginMetadata(
    Id = "RSession",
    Version = "1.1.0",
    Name = "RSession",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed class RSession(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;

    private IDatabaseFactory? _databaseFactory;
    private IEventService? _eventService;

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

        interfaceManager.AddSharedInterface<ISessionMapService, MapService>(
            "RSession.MapService",
            _serviceProvider.GetRequiredService<MapService>()
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
        _databaseFactory?.InvokeDatabaseConfigured();

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

        _databaseFactory = _serviceProvider.GetRequiredService<IDatabaseFactory>();
        _eventService = _serviceProvider.GetRequiredService<IEventService>();

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
        _eventService?.InvokeDispose();

        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
    }
}
