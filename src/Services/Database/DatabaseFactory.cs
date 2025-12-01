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
using Microsoft.Extensions.Options;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using RSession.Models.Config;

namespace RSession.Services.Database;

internal sealed class DatabaseFactory : IDatabaseFactory
{
    private readonly ILogService _logService;
    private readonly ILogger<DatabaseFactory> _logger;
    private readonly IOptionsMonitor<DatabaseConfig> _config;

    private readonly IEventService _eventService;
    private readonly IDatabaseService _databaseService;

    public DatabaseFactory(
        ILogService logService,
        ILogger<DatabaseFactory> logger,
        IOptionsMonitor<DatabaseConfig> config,
        IEventService eventService,
        Lazy<IPostgresService> postgresService,
        Lazy<ISqlService> sqlService
    )
    {
        _logService = logService;
        _logger = logger;
        _config = config;

        _eventService = eventService;

        string type = _config.CurrentValue.Type;

        _databaseService = type.ToLowerInvariant() switch
        {
            "postgres" => postgresService.Value,
            "mysql" => sqlService.Value,
            _ => throw _logService.LogCritical(
                $"Database is not supported - '{type}' | Supported types: postgres, mysql",
                logger: _logger
            ),
        };

        _logService.LogInformation($"DatabaseFactory initialized - '{type}'", logger: _logger);
    }

    public IDatabaseService GetDatabaseService() => _databaseService;

    public void InvokeDatabaseConfigured() =>
        _eventService.InvokeDatabaseConfigured(
            _databaseService,
            _config.CurrentValue.Type,
            _config.CurrentValue.Prefix
        );
}
