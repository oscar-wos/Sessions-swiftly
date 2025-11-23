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
using RSession.Played.Contracts.Database;
using RSession.Played.Models.Database;
using RSession.Shared.Contracts.Database;

namespace RSession.Played.Services.Database;

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

    public async Task InsertPlayedAsync(long sessionId)
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

        await using NpgsqlCommand command = new(_queries.InsertPlayed, connection);
        _ = command.Parameters.AddWithValue("@sessionId", sessionId);

        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task UpdatePlayedAsync(
        long[] aliveT,
        long[] aliveCT,
        long[] deadT,
        long[] deadCT,
        long[] spec,
        int interval
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

        await using NpgsqlTransaction transaction = await connection
            .BeginTransactionAsync()
            .ConfigureAwait(false);

        if (aliveT.Length > 0)
        {
            await using NpgsqlCommand command = new(
                _queries.UpdatePlayedAliveT,
                connection,
                transaction
            );

            _ = command.Parameters.AddWithValue("@sessionIds", aliveT);
            _ = command.Parameters.AddWithValue("@interval", interval);

            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        if (aliveCT.Length > 0)
        {
            await using NpgsqlCommand command = new(
                _queries.UpdatePlayedAliveCT,
                connection,
                transaction
            );

            _ = command.Parameters.AddWithValue("@sessionIds", aliveCT);
            _ = command.Parameters.AddWithValue("@interval", interval);

            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        if (deadT.Length > 0)
        {
            await using NpgsqlCommand command = new(
                _queries.UpdatePlayedDeadT,
                connection,
                transaction
            );

            _ = command.Parameters.AddWithValue("@sessionIds", deadT);
            _ = command.Parameters.AddWithValue("@interval", interval);

            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        if (deadCT.Length > 0)
        {
            await using NpgsqlCommand command = new(
                _queries.UpdatePlayedDeadCT,
                connection,
                transaction
            );

            _ = command.Parameters.AddWithValue("@sessionIds", deadCT);
            _ = command.Parameters.AddWithValue("@interval", interval);

            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        if (spec.Length > 0)
        {
            await using NpgsqlCommand command = new(
                _queries.UpdatePlayedSpec,
                connection,
                transaction
            );

            _ = command.Parameters.AddWithValue("@sessionIds", spec);
            _ = command.Parameters.AddWithValue("@interval", interval);

            _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        await transaction.CommitAsync().ConfigureAwait(false);
    }
}
