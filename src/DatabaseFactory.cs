using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Log;

namespace Sessions;

internal class DatabaseFactory : IDatabaseFactory, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogService _logService;
    private readonly ILogger<DatabaseFactory> _logger;

    public IDatabaseService Database { get; }

    public DatabaseFactory(
        string type,
        IServiceProvider serviceProvider,
        ILogService logService,
        ILogger<DatabaseFactory> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logService = logService;
        _logger = logger;

        Database = type.ToLowerInvariant() switch
        {
            "postgres" => (IDatabaseService)_serviceProvider.GetRequiredService<IPostgresService>(),
            "mysql" => (IDatabaseService)_serviceProvider.GetRequiredService<ISqlService>(),
            _ => throw _logService.LogCritical(
                $"Database type '{type}' is not supported. Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _logService.LogInformation(
            $"Database factory initialized with {type} service",
            logger: _logger
        );
    }

    public void Dispose() => (Database as IDisposable)?.Dispose();
}
