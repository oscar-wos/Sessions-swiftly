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
using MySqlConnector;
using RSession.Aliases.Contracts.Database;
using RSession.Aliases.Models.Database;
using RSession.Shared.Contracts.Database;

namespace RSession.Aliases.Services.Database;

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
            await _sessionDatabaseService.GetConnectionAsync().ConfigureAwait(false)
            as MySqlConnection;

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

    public async Task<string?> SelectAliasAsync(int playerId)
    {
        if (_sessionDatabaseService is null)
        {
            return null;
        }

        await using MySqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync().ConfigureAwait(false)
            as MySqlConnection;

        if (connection is null)
        {
            return null;
        }

        await using MySqlCommand command = new(_queries.SelectAlias, connection);
        _ = command.Parameters.AddWithValue("@playerId", playerId);

        await using MySqlDataReader reader = await command
            .ExecuteReaderAsync()
            .ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            return reader.GetString(0);
        }

        return null;
    }

    public async Task InsertAliasAsync(int playerId, string alias)
    {
        if (_sessionDatabaseService is null)
        {
            return;
        }

        await using MySqlConnection? connection =
            await _sessionDatabaseService.GetConnectionAsync().ConfigureAwait(false)
            as MySqlConnection;

        if (connection is null)
        {
            return;
        }

        await using MySqlCommand command = new(_queries.InsertAlias, connection);

        _ = command.Parameters.AddWithValue("@playerId", playerId);
        _ = command.Parameters.AddWithValue("@alias", alias);

        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
