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
using RSession.Messages.Contracts.Database;

namespace RSession.Messages.Models.Database;

internal sealed class PostgresQueries(string prefix) : LoadQueries, IDatabaseQueries
{
    private readonly string _prefix = prefix;

    protected override string CreateMessages =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}messages (
                id BIGSERIAL PRIMARY KEY,
                session_id BIGINT NOT NULL,
                team_num SMALLINT NOT NULL,
                team_chat BOOLEAN NOT NULL,
                message TEXT NOT NULL,
                timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;

    public string InsertMessage =>
        $"INSERT INTO {_prefix}messages (session_id, team_num, team_chat, message) VALUES (@sessionId, @teamNum, @teamChat, @message)";
}
