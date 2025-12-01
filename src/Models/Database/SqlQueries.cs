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

public class SqlQueries(string prefix) : LoadQueries, IDatabaseQueries
{
    private readonly string _prefix = prefix;

    protected override string CreatePlayers =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}players (
                id INT AUTO_INCREMENT,
                steam_id BIGINT NOT NULL PRIMARY KEY,
                first_seen DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                last_seen DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
            )
            """;

    protected override string CreateServers =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}servers (
                id SMALLINT AUTO_INCREMENT PRIMARY KEY,
                ip VARCHAR(15) NOT NULL,
                port SMALLINT UNSIGNED NOT NULL
            )
            """;

    protected override string CreateSessions =>
        $"""
            CREATE TABLE IF NOT EXISTS {_prefix}sessions (
                id BIGINT AUTO_INCREMENT PRIMARY KEY,
                player_id INT NOT NULL,
                server_id SMALLINT NOT NULL,
                ip VARCHAR(15) NOT NULL,
                start_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                end_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            );

            CREATE INDEX IF NOT EXISTS idx_{_prefix}sessions_player_id ON {_prefix}sessions(player_id);
            CREATE INDEX IF NOT EXISTS idx_{_prefix}sessions_server_id ON {_prefix}sessions(server_id)
            """;

    public string SelectPlayer => $"SELECT id FROM {_prefix}players WHERE steam_id = @steamId";

    public string InsertPlayer =>
        $"INSERT INTO {_prefix}players (steam_id) VALUES (@steamId); SELECT LAST_INSERT_ID()";

    public string SelectServer =>
        $"SELECT id FROM {_prefix}servers WHERE ip = @ip AND port = @port";

    public string InsertServer =>
        $"INSERT INTO {_prefix}servers (ip, port) VALUES (@ip, @port); SELECT LAST_INSERT_ID()";

    public string InsertSession =>
        $"INSERT INTO {_prefix}sessions (player_id, server_id, ip) VALUES (@playerId, @serverId, @ip); SELECT LAST_INSERT_ID()";

    public string UpdateSeen =>
        $"UPDATE {_prefix}players SET last_seen = NOW() WHERE id IN (@playerIds)";

    public string UpdateSession =>
        $"UPDATE {_prefix}sessions SET end_time = NOW() WHERE id IN (@sessionIds)";
}
