using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Log;
using RSession.API.Models.Config;

namespace RSession.Services.Database;

public sealed class DatabaseFactory : IDatabaseFactory
{
    private readonly ILogService _logService;
    private readonly ILogger<DatabaseFactory> _logger;
    private readonly IOptionsMonitor<DatabaseConfig> _config;

    private readonly IServiceProvider _serviceProvider;

    public IDatabaseService Database { get; private set; }

    public DatabaseFactory(
        ILogService logService,
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
