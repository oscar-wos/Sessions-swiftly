using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using RSession.Models.Config;
using RSession.Models.Database;

namespace RSession.Services.Database;

internal sealed class PostgresService : IPostgresService, IAsyncDisposable
{
    private readonly ILogService _logService;
    private readonly ILogger<PostgresService> _logger;
    private readonly IOptionsMonitor<DatabaseConfig> _config;

    private readonly NpgsqlDataSource _dataSource;
    private readonly PostgresQueries _queries;

    public PostgresService(
        ILogService logService,
        ILogger<PostgresService> logger,
        IOptionsMonitor<DatabaseConfig> config
    )
    {
        _logService = logService;
        _logger = logger;
        _config = config;

        string connectionString = BuildConnectionString(_config.CurrentValue.Connection);
        _dataSource = NpgsqlDataSource.Create(connectionString);
        _queries = new PostgresQueries();
    }

    public async Task<DbConnection> GetConnectionAsync() =>
        await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await using NpgsqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using NpgsqlTransaction transaction = await connection
            .BeginTransactionAsync()
            .ConfigureAwait(false);

        foreach (string query in _queries.GetLoadQueries())
        {
            await using NpgsqlCommand command = new(query, connection, transaction);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        await transaction.CommitAsync().ConfigureAwait(false);
    }

    public async Task<int> GetPlayerAsync(ulong steamId)
    {
        await using NpgsqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (NpgsqlCommand command = new(_queries.SelectPlayer, connection))
        {
            _ = command.Parameters.AddWithValue("@steamId", (long)steamId);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int result)
            {
                return result;
            }
        }

        await using (NpgsqlCommand command = new(_queries.InsertPlayer, connection))
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
        await using NpgsqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (NpgsqlCommand command = new(_queries.SelectServer, connection))
        {
            _ = command.Parameters.AddWithValue("@ip", ip);
            _ = command.Parameters.AddWithValue("@port", (short)port);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is short result)
            {
                return result;
            }
        }

        await using (NpgsqlCommand command = new(_queries.InsertServer, connection))
        {
            _ = command.Parameters.AddWithValue("@ip", ip);
            _ = command.Parameters.AddWithValue("@port", (short)port);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is not short result)
            {
                throw new Exception("Failed to insert server");
            }

            return result;
        }
    }

    public async Task<long> GetSessionAsync(int playerId, short serverId, string ip)
    {
        await using NpgsqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using NpgsqlCommand command = new(_queries.InsertSession, connection);

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
        await using NpgsqlConnection connection = await _dataSource
            .OpenConnectionAsync()
            .ConfigureAwait(false);

        await using (NpgsqlCommand command = new(_queries.UpdateSeen, connection))
        {
            _ = command.Parameters.AddWithValue("@playerIds", playerIds);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        await using (NpgsqlCommand command = new(_queries.UpdateSession, connection))
        {
            _ = command.Parameters.AddWithValue("@sessionIds", sessionIds);
            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

    private string BuildConnectionString(ConnectionConfig config)
    {
        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = config.Host,
            Port = config.Port,
            Username = config.Username,
            Password = config.Password,
            Database = config.Database,
            Pooling = true,
        };

        string connectionString = builder.ConnectionString;
        _logService.LogDebug(connectionString, logger: _logger);

        return builder.ConnectionString;
    }

    public async ValueTask DisposeAsync()
    {
        await _dataSource.DisposeAsync().ConfigureAwait(false);
        _logService.LogInformation("PostgresService disposed", logger: _logger);
    }
}
