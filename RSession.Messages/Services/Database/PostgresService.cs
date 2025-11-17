using Npgsql;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Models.Database;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Database;

internal class PostgresService : IDatabaseService
{
    private readonly PostgresQueries _queries = new();

    private ISessionDatabaseService? _databaseService;

    public void Initialize(ISessionDatabaseService databaseService) =>
        _databaseService = databaseService;

    public async Task InitAsync()
    {
        if (_databaseService is null)
        {
            return;
        }

        await using NpgsqlConnection? connection =
            await _databaseService.GetConnectionAsync() as NpgsqlConnection;

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
}
