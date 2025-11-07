using Microsoft.Extensions.DependencyInjection;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Hook;
using Sessions.Services.Database;
using Sessions.Services.Hook;

namespace Sessions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
        _ = services.AddSingleton<IPostgresService, PostgresService>();
        _ = services.AddSingleton<ISqlService, SqlService>();

        return services;
    }

    public static IServiceCollection AddHooks(this IServiceCollection services)
    {
        _ = services.AddSingleton<IHookManager, IHookManager>();
        _ = services.AddSingleton<IPlayerConnectService, PlayerConnectService>();
        _ = services.AddSingleton<IPlayerMessageService, PlayerMessageService>();

        return services;
    }
}
