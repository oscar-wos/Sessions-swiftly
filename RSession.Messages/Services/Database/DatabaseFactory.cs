using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Log;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Database;

internal class DatabaseFactory(
    ILogService logService,
    ILogger<DatabaseFactory> logger,
    PostgresService postgresService,
    SqlService sqlService
) : IDatabaseFactory
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<DatabaseFactory> _logger = logger;

    private readonly PostgresService _postgresService = postgresService;
    private readonly SqlService _sqlService = sqlService;

    private IDatabaseService? _databaseService;

    public void RegisterDatabaseService(ISessionDatabaseService databaseService, string type)
    {
        Console.WriteLine("regiserting database");

        _databaseService = type.ToLowerInvariant() switch
        {
            "postgres" => _postgresService,
            "mysql" => _sqlService,
            _ => throw _logService.LogCritical(
                $"Database is not supported - '{type}' | Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _databaseService.Initialize(databaseService);
        _logService.LogInformation($"DatabaseFactory initialized - '{type}'", logger: _logger);

        _ = Task.Run(async () => await _databaseService.InitAsync());
    }
}
