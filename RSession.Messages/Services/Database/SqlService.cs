using MySqlConnector;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Models.Database;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Database;

internal sealed class SqlService : ISqlService
{
    private readonly SqlQueries _queries = new();

    private ISessionDatabaseService? _sessionDatabaseService;

    public void Initialize(ISessionDatabaseService sessionDatabaseService) =>
        _sessionDatabaseService = sessionDatabaseService;

    public async Task CreateTablesAsync()
    {
        if (_sessionDatabaseService is null)
        {
            return;
        }

        await using MySqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync() as MySqlConnection;

        if (connection is null)
        {
            return;
        }

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

    public async Task InsertMessageAsync(
        long sessionId,
        short teamNum,
        bool teamChat,
        string message
    )
    {
        if (_sessionDatabaseService is null)
        {
            return;
        }

        await using MySqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync() as MySqlConnection;

        if (connection is null)
        {
            return;
        }

        await using MySqlCommand command = new(_queries.InsertMessage, connection);

        _ = command.Parameters.AddWithValue("@sessionId", sessionId);
        _ = command.Parameters.AddWithValue("@teamNum", teamNum);
        _ = command.Parameters.AddWithValue("@teamChat", teamChat);
        _ = command.Parameters.AddWithValue("@message", message);

        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
