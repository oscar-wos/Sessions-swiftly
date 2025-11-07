using Microsoft.Extensions.DependencyInjection;
using Sessions.API.Contracts.Database;
using Sessions.Services.Database;

namespace Sessions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        _ = services.AddSingleton<IPostgresService, PostgresService>();
        _ = services.AddSingleton<ISqlService, SqlService>();
        _ = services.AddSingleton<IDatabaseFactory, DatabaseFactory>();

        return services;
    }
}
