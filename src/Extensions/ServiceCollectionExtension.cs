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
using RSession.Contracts.Log;
using RSession.Contracts.Schedule;
using RSession.Models.Config;
using RSession.Services.Core;
using RSession.Services.Database;
using RSession.Services.Event;
using RSession.Services.Log;
using RSession.Services.Schedule;

namespace RSession.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConfigs(this IServiceCollection services)
    {
        _ = services.AddOptionsWithValidateOnStart<SessionConfig>().BindConfiguration("config");
        _ = services.AddOptionsWithValidateOnStart<DatabaseConfig>().BindConfiguration("database");

        return services;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection services)
    {
        _ = services.AddSingleton<PostgresService>();
        _ = services.AddSingleton<SqlService>();

        _ = services.AddSingleton(serviceProvider => new Lazy<IPostgresService>(() =>
            serviceProvider.GetRequiredService<PostgresService>()
        ));

        _ = services.AddSingleton(serviceProvider => new Lazy<ISqlService>(() =>
            serviceProvider.GetRequiredService<SqlService>()
        ));

        _ = services.AddSingleton<IDatabaseFactory, DatabaseFactory>();

        return services;
    }

    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        _ = services.AddSingleton<IEventListener, OnClientDisconnectedService>();
        _ = services.AddSingleton<IEventListener, OnClientSteamAuthorizeService>();
        _ = services.AddSingleton<IEventListener, OnClientSteamAuthorizeFailService>();
        _ = services.AddSingleton<IEventListener, OnMapLoadService>();
        _ = services.AddSingleton<IEventListener, OnSteamAPIActivatedService>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IIntervalService, IntervalService>();

        _ = services.AddSingleton<EventService>();
        _ = services.AddSingleton<MapService>();
        _ = services.AddSingleton<PlayerService>();
        _ = services.AddSingleton<ServerService>();

        _ = services.AddSingleton<IEventService>(serviceProvider =>
            serviceProvider.GetRequiredService<EventService>()
        );

        _ = services.AddSingleton<IMapService>(serviceProvider =>
            serviceProvider.GetRequiredService<MapService>()
        );

        _ = services.AddSingleton<IPlayerService>(serviceProvider =>
            serviceProvider.GetRequiredService<PlayerService>()
        );

        _ = services.AddSingleton<IServerService>(serviceProvider =>
            serviceProvider.GetRequiredService<ServerService>()
        );

        return services;
    }
}
