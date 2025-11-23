// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Npgsql;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Models.Database;
using RSession.Shared.Contracts.Database;

namespace RSession.Messages.Services.Database;

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
            await _sessionDatabaseService.GetConnectionAsync().ConfigureAwait(false)
            as NpgsqlConnection;

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
            await _sessionDatabaseService.GetConnectionAsync().ConfigureAwait(false)
            as NpgsqlConnection;

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
