using Microsoft.Extensions.DependencyInjection;
using Sessions.API.Contracts.Database;
using Sessions.Services.Database;

namespace Sessions.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddDatabase(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDatabaseService, PostgresService>();
        _ = services.AddSingleton<IDatabaseService, SqlService>();

        _ = services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
    }
}
