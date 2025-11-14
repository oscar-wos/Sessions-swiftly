using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RSession.Contracts.Database;
using RSession.Models.Config;
using RSession.Shared.Contracts.Log;

namespace RSession.Services.Database;

internal sealed class DatabaseFactory : IDatabaseFactory
{
    private readonly IRSessionLog _logService;
    private readonly ILogger<DatabaseFactory> _logger;
    private readonly IOptionsMonitor<DatabaseConfig> _config;

    private readonly IServiceProvider _serviceProvider;

    public IDatabaseService Database { get; private set; }

    public DatabaseFactory(
        IRSessionLog logService,
        ILogger<DatabaseFactory> logger,
        IOptionsMonitor<DatabaseConfig> config,
        IServiceProvider serviceProvider
    )
    {
        _logService = logService;
        _logger = logger;
        _config = config;

        _serviceProvider = serviceProvider;

        string type = _config.CurrentValue.Type;

        Database = type.ToLowerInvariant() switch
        {
            "postgres" => (IDatabaseService)_serviceProvider.GetRequiredService<IPostgresService>(),
            "mysql" => (IDatabaseService)_serviceProvider.GetRequiredService<ISqlService>(),
            _ => throw _logService.LogCritical(
                $"Database is not supported - '{type}' | Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _logService.LogInformation($"DatabaseFactory initialized - '{type}'", logger: _logger);
    }
}
