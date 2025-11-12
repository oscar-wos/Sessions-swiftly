using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Log;
using RSession.API.Models.Config;
using RSession.Models;

namespace RSession.Services.Database;

public sealed class SqlService : ISqlService, IDatabaseService
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

        _queries = new SqlQueries();
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

    public async Task InitAsync()
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
            _ = command.Parameters.AddWithValue("@port", (short)port);

            if (await command.ExecuteScalarAsync().ConfigureAwait(false) is short result)
            {
                return result;
            }
        }

        await using (MySqlCommand command = new(_queries.InsertServer, connection))
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
}
