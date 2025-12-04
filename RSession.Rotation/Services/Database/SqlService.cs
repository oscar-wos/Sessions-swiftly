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
using RSession.Rotation.Contracts.Database;
using RSession.Rotation.Models.Database;
using RSession.Shared.Contracts.Database;

namespace RSession.Rotation.Services.Database;

internal sealed class SqlService : ISqlService
{
    private ISessionDatabaseService? _sessionDatabaseService;
    private SqlQueries? _queries;

    public void Initialize(ISessionDatabaseService sessionDatabaseService, string prefix)
    {
        _sessionDatabaseService = sessionDatabaseService;
        _queries = new SqlQueries(prefix);
    }

    public async Task CreateTablesAsync()
    {
        if (_sessionDatabaseService is null || _queries is null)
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

    public async Task InsertRotationAsync(short serverId, short mapId)
    {
        if (_sessionDatabaseService is null || _queries is null)
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

        await using MySqlCommand command = new(_queries.InsertRotation, connection);

        _ = command.Parameters.AddWithValue("@serverId", serverId);
        _ = command.Parameters.AddWithValue("@mapId", mapId);

        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
