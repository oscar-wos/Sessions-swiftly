using Microsoft.Extensions.DependencyInjection;
using RSession.Maps.Contracts.Core;
using RSession.Maps.Contracts.Database;
using RSession.Maps.Contracts.Event;
using RSession.Maps.Contracts.Hook;
using RSession.Maps.Contracts.Log;
using RSession.Maps.Services.Core;
using RSession.Maps.Services.Database;
using RSession.Maps.Services.Event;
using RSession.Maps.Services.Hook;
using RSession.Maps.Services.Log;

namespace RSession.Maps.Extensions;

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

        return services;
    }

    public static IServiceCollection AddHooks(this IServiceCollection services)
    {
        _ = services.AddSingleton<IHook, OnUserMessageSayText2Service>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<ILogService, LogService>();
        _ = services.AddSingleton<IPlayerService, PlayerService>();

        return services;
    }
}
