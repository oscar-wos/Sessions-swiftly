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
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using RSession.Models.Config;
using RSession.Models.Database;

namespace RSession.Services.Database;

internal sealed class SqlService : ISqlService, IAsyncDisposable
{
    private readonly ILogService _logService;
    private readonly ILogger<SqlService> _logger;
    private readonly IOptionsMonitor<DatabaseConfig> _config;

    private readonly MySqlDataSource _dataSource;
    private readonly SqlQueries _queries;

    public SqlService(
        ILogService logService,
        ILogger<SqlService> logger,
        IOptionsMonitor<DatabaseConfig> config
    )
    {
        _logService = logService;
        _logger = logger;
        _config = config;

        string connectionString = BuildConnectionString(_config.CurrentValue.Connection);
        _dataSource = new MySqlDataSourceBuilder(connectionString).Build();
        _queries = new SqlQueries(_config.CurrentValue.Prefix);
    }

    public async Task<DbConnection> GetConnectionAsync() =>
        await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

    public async Task CreateTablesAsync()
    {
        await using MySqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using MySqlTransaction transaction = await connection
            .BeginTransactionAsync()
            .ConfigureAwait(false);

        foreach (string query in _queries.GetLoadQueries())
        {
            await using MySqlCommand command = new(query, connection, transaction);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        await transaction.CommitAsync().ConfigureAwait(false);
    }

    public async Task<int> GetPlayerAsync(ulong steamId)
    {
        await using MySqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (MySqlCommand command = new(_queries.SelectPlayer, connection))
        {
            _ = command.Parameters.AddWithValue("@steamId", (long)steamId);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int result)
            {
                return result;
            }
        }

        await using (MySqlCommand command = new(_queries.InsertPlayer, connection))
        {
            _ = command.Parameters.AddWithValue("@steamId", (long)steamId);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is not int result)
            {
                throw new Exception("Failed to insert player");
            }

            return result;
        }
    }

    public async Task<short> GetServerAsync(string ip, ushort port)
    {
        await using MySqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (MySqlCommand command = new(_queries.SelectServer, connection))
        {
            _ = command.Parameters.AddWithValue("@ip", ip);
            _ = command.Parameters.AddWithValue("@port", port);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is short result)
            {
                return result;
            }
        }

        await using (MySqlCommand command = new(_queries.InsertServer, connection))
        {
            _ = command.Parameters.AddWithValue("@ip", ip);
            _ = command.Parameters.AddWithValue("@port", port);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is not short result)
            {
                throw new Exception("Failed to insert server");
            }

            return result;
        }
    }

    public async Task<long> GetSessionAsync(int playerId, short serverId, string ip)
    {
        await using MySqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using MySqlCommand command = new(_queries.InsertSession, connection);

        _ = command.Parameters.AddWithValue("@playerId", playerId);
        _ = command.Parameters.AddWithValue("@serverId", serverId);
        _ = command.Parameters.AddWithValue("@ip", ip);

        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is not long result)
        {
            throw new Exception("Failed to insert session");
        }

        return result;
    }

    public async Task UpdateSessionsAsync(List<int> playerIds, List<long> sessionIds)
    {
        await using MySqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (MySqlCommand command = new(_queries.UpdateSeen, connection))
        {
            _ = command.Parameters.AddWithValue("@playerIds", playerIds);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        await using (MySqlCommand command = new(_queries.UpdateSession, connection))
        {
            _ = command.Parameters.AddWithValue("@sessionIds", sessionIds);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

    private string BuildConnectionString(ConnectionConfig config)
    {
        MySqlConnectionStringBuilder builder = new()
        {
            Server = config.Host,
            Port = (uint)config.Port,
            UserID = config.Username,
            Password = config.Password,
            Database = config.Database,
            Pooling = true,
        };

        string connectionString = builder.ConnectionString;
        _logService.LogDebug(connectionString, logger: _logger);

        return builder.ConnectionString;
    }

    public async ValueTask DisposeAsync() => await _dataSource.DisposeAsync().ConfigureAwait(false);
}
