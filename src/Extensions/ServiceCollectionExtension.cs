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
using RSession.Shared.Contracts;

namespace RSession.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConfigs(this IServiceCollection services)
    {
        _ = services.AddOptionsWithValidateOnStart<DatabaseConfig>().BindConfiguration("database");
        _ = services.AddOptionsWithValidateOnStart<SessionConfig>().BindConfiguration("config");

        return services;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
        _ = services.AddSingleton<IPostgresService, PostgresService>();
        _ = services.AddSingleton<ISqlService, SqlService>();

        return services;
    }

    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        _ = services.AddSingleton<IEventListener, OnClientDisconnectedService>();
        _ = services.AddSingleton<IEventListener, OnClientSteamAuthorizeService>();
        _ = services.AddSingleton<IEventListener, OnSteamAPIActivatedService>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<EventService>();
        _ = services.AddSingleton<PlayerService>();
        _ = services.AddSingleton<ServerService>();

        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IInterval, IntervalService>();

        _ = services.AddSingleton<IRSessionEventInternal>(serviceProvider =>
            serviceProvider.GetRequiredService<EventService>()
        );

        _ = services.AddSingleton<IRSessionEvent>(serviceProvider =>
            serviceProvider.GetRequiredService<EventService>()
        );

        _ = services.AddSingleton<IRSessionPlayerInternal>(serviceProvider =>
            serviceProvider.GetRequiredService<PlayerService>()
        );

        _ = services.AddSingleton<IRSessionPlayer>(serviceProvider =>
            serviceProvider.GetRequiredService<PlayerService>()
        );

        _ = services.AddSingleton<IRSessionServerInternal>(serviceProvider =>
            serviceProvider.GetRequiredService<ServerService>()
        );

        _ = services.AddSingleton<IRSessionServer>(serviceProvider =>
            serviceProvider.GetRequiredService<ServerService>()
        );

        return services;
    }
}
