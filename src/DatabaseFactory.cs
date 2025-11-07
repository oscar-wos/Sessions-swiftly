using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Log;
using Sessions.API.Models.Config;

namespace Sessions;

internal sealed class DatabaseFactory : IDatabaseFactory, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<DatabaseConfig> _config;
    private readonly ILogService _logService;
    private readonly ILogger<DatabaseFactory> _logger;

    public IDatabaseService Database { get; }

    public DatabaseFactory(
        IServiceProvider services,
        IOptionsMonitor<DatabaseConfig> config,
        ILogService logService,
        ILogger<DatabaseFactory> logger
    )
    {
        _services = services;
        _config = config;
        _logService = logService;
        _logger = logger;

        string type = _config.CurrentValue.Type;

        Database = type.ToLowerInvariant() switch
        {
            "postgres" => (IDatabaseService)_services.GetRequiredService<IPostgresService>(),
            "mysql" => (IDatabaseService)_services.GetRequiredService<ISqlService>(),
            _ => throw _logService.LogCritical(
                $"Database type '{type}' is not supported. Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _logService.LogInformation($"Database factory initialized with '{type}'", logger: _logger);
    }

    public void Dispose() => (Database as IDisposable)?.Dispose();
}
