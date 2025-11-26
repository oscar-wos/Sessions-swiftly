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
using RSession.Played.Contracts.Core;
using RSession.Played.Contracts.Database;
using RSession.Played.Contracts.Event;
using RSession.Played.Contracts.Log;
using RSession.Played.Services.Core;
using RSession.Played.Services.Database;
using RSession.Played.Services.Event;
using RSession.Played.Services.Log;
using RSession.Shared.Contracts.Event;

namespace RSession.Played.Extensions;

public static class ServiceCollectionExtension
{
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
        _ = services.AddSingleton<IOnDatabaseConfiguredService, OnDatabaseConfiguredService>();
        _ = services.AddSingleton<IOnElapsedService, OnElapsedService>();
        _ = services.AddSingleton<IOnPlayerRegisteredService, OnPlayerRegisteredService>();

        _ = services.AddSingleton<ISessionEventListener>(serviceProvider =>
            serviceProvider.GetRequiredService<IOnDatabaseConfiguredService>()
        );

        _ = services.AddSingleton<ISessionEventListener>(serviceProvider =>
            serviceProvider.GetRequiredService<IOnElapsedService>()
        );

        _ = services.AddSingleton<ISessionEventListener>(serviceProvider =>
            serviceProvider.GetRequiredService<IOnPlayerRegisteredService>()
        );

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IPlayerService, PlayerService>();

        return services;
    }
}
