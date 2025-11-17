using Microsoft.Extensions.Logging;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Log;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Database;

internal sealed class DatabaseFactory(
    ILogService logService,
    ILogger<DatabaseFactory> logger,
    Lazy<IPostgresService> postgresService,
    Lazy<ISqlService> sqlService
) : IDatabaseFactory
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<DatabaseFactory> _logger = logger;

    private readonly Lazy<IPostgresService> _postgresService = postgresService;
    private readonly Lazy<ISqlService> _sqlService = sqlService;

    private IDatabaseService? _databaseService;

    public IDatabaseService? GetDatabaseService() => _databaseService;

    public void RegisterDatabaseService(ISessionDatabaseService sessionDatabaseService, string type)
    {
        _databaseService = type.ToLowerInvariant() switch
        {
            "postgres" => _postgresService.Value,
            "mysql" => _sqlService.Value,
            _ => throw _logService.LogCritical(
                $"Database is not supported - '{type}' | Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _databaseService.Initialize(sessionDatabaseService);
        _ = Task.Run(async () => await _databaseService.CreateTablesAsync());

        _logService.LogInformation($"DatabaseFactory initialized - '{type}'", logger: _logger);
    }
}
