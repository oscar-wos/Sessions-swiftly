using Microsoft.Extensions.DependencyInjection;
using RSession.Messages.Contracts.Core;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Event;
using RSession.Messages.Contracts.Hook;
using RSession.Messages.Contracts.Log;
using RSession.Messages.Services.Core;
using RSession.Messages.Services.Database;
using RSession.Messages.Services.Event;
using RSession.Messages.Services.Hook;
using RSession.Messages.Services.Log;

namespace RSession.Messages.Extensions;

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
