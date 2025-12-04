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
using RSession.Contracts.Database;

namespace RSession.Models.Database;

public class PostgresQueries(string prefix) : LoadQueries, IDatabaseQueries
{
    private readonly string _prefix = prefix;

    protected override string CreateMaps =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}maps (
                id SMALLSERIAL PRIMARY KEY,
                name VARCHAR(64) NOT NULL UNIQUE,
                workshop_id BIGINT
            )
            """;

    protected override string CreatePlayers =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}players (
                id SERIAL,
                steam_id BIGINT NOT NULL PRIMARY KEY,
                first_seen TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                last_seen TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;

    protected override string CreateServers =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}servers (
                id SMALLSERIAL PRIMARY KEY,
                ip INET NOT NULL,
                port SMALLINT NOT NULL
            )
            """;

    protected override string CreateSessions =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}sessions (
                id BIGSERIAL PRIMARY KEY,
                player_id INT NOT NULL,
                server_id SMALLINT NOT NULL,
                ip INET NOT NULL,
                start_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                end_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            );

            CREATE INDEX IF NOT EXISTS idx_{_prefix}sessions_player_id ON {_prefix}sessions(player_id);
            CREATE INDEX IF NOT EXISTS idx_{_prefix}sessions_server_id ON {_prefix}sessions(server_id)
            """;

    public string SelectMap => $"SELECT id from {_prefix}maps WHERE name = @name";

    public string InsertMap =>
        $"INSERT INTO {_prefix}maps (name, workshop_id) VALUES (@name, @workshopId) RETURNING id";

    public string SelectPlayer => $"SELECT id FROM {_prefix}players WHERE steam_id = @steamId";

    public string InsertPlayer =>
        $"INSERT INTO {_prefix}players (steam_id) VALUES (@steamId) RETURNING id";

    public string SelectServer =>
        $"SELECT id FROM {_prefix}servers WHERE ip = CAST(@ip as INET) AND port = @port";

    public string InsertServer =>
        $"INSERT INTO {_prefix}servers (ip, port) VALUES (CAST(@ip as INET), @port) RETURNING id";

    public string InsertSession =>
        $"INSERT INTO {_prefix}sessions (player_id, server_id, ip) VALUES (@playerId, @serverId, CAST(@ip as INET)) RETURNING id";

    public string UpdateSeen =>
        $"UPDATE {_prefix}players SET last_seen = NOW() WHERE id = ANY(@playerIds)";

    public string UpdateSession =>
        $"UPDATE {_prefix}sessions SET end_time = NOW() WHERE id = ANY(@sessionIds)";
}
