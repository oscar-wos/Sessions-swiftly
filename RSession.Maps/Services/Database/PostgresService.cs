using Npgsql;
using RSession.Maps.Contracts.Database;
using RSession.Maps.Models.Database;
using RSession.Shared.Contracts;

namespace RSession.Maps.Services.Database;

internal sealed class PostgresService : IPostgresService
{
    private readonly PostgresQueries _queries = new();

    private ISessionDatabaseService? _sessionDatabaseService;

    public void Initialize(ISessionDatabaseService sessionDatabaseService) =>
        _sessionDatabaseService = sessionDatabaseService;

    public async Task CreateTablesAsync()
    {
        if (_sessionDatabaseService is null)
        {
            return;
        }

        await using NpgsqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync() as NpgsqlConnection;

        if (connection is null)
        {
            return;
        }

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

        await using NpgsqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync() as NpgsqlConnection;

        if (connection is null)
        {
            return;
        }

        await using NpgsqlCommand command = new(_queries.InsertMessage, connection);

        _ = command.Parameters.AddWithValue("@sessionId", sessionId);
        _ = command.Parameters.AddWithValue("@teamNum", teamNum);
        _ = command.Parameters.AddWithValue("@teamChat", teamChat);
        _ = command.Parameters.AddWithValue("@message", message);

        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
