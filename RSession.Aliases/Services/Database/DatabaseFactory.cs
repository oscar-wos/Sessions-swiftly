// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.Logging;
using RSession.Aliases.Contracts.Database;
using RSession.Aliases.Contracts.Log;
using RSession.Shared.Contracts.Database;

namespace RSession.Aliases.Services.Database;

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

    public void Initialize(ISessionDatabaseService sessionDatabaseService, string type)
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
        _ = Task.Run(async () => await _databaseService.CreateTablesAsync().ConfigureAwait(false));

        _logService.LogInformation($"DatabaseFactory initialized - '{type}'", logger: _logger);
    }
}
