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
using RSession.Rotation.Contracts.Database;

namespace RSession.Rotation.Models.Database;

internal sealed class PostgresQueries(string prefix) : LoadQueries, IDatabaseQueries
{
    private readonly string _prefix = prefix;

    protected override string CreateRotation =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}rotation (
                id SERIAL PRIMARY KEY,
                server_id SMALLINT NOT NULL,
                map_id SMALLINT NOT NULL,
                timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;

    public string InsertRotation =>
        $"INSERT INTO {_prefix}rotation (server_id, map_id) VALUES (@serverId, @mapId)";
}
