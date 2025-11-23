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
using RSession.Played.Contracts.Database;

namespace RSession.Played.Models.Database;

internal sealed class PostgresQueries : LoadQueries, IDatabaseQueries
{
    protected override string CreatePlayed =>
        """
            CREATE TABLE IF NOT EXISTS played (
                id BIGSERIAL PRIMARY KEY,
                session_id BIGINT NOT NULL,
                alive_t INT NOT NULL DEFAULT 0,
                alive_ct INT NOT NULL DEFAULT 0,
                dead_t INT NOT NULL DEFAULT 0,
                dead_ct INT NOT NULL DEFAULT 0,
                spec INT NOT NULL DEFAULT 0
            )
            """;

    public string InsertPlayed => "INSERT INTO played (session_id) VALUES (@sessionId)";

    public string UpdatePlayedAliveT =>
        "UPDATE played SET alive_t = alive_t + @interval WHERE session_id = ANY(@sessionIds)";

    public string UpdatePlayedAliveCT =>
        "UPDATE played SET alive_ct = alive_ct + @interval WHERE session_id = ANY(@sessionIds)";

    public string UpdatePlayedDeadT =>
        "UPDATE played SET dead_t = dead_t + @interval WHERE session_id = ANY(@sessionIds)";

    public string UpdatePlayedDeadCT =>
        "UPDATE played SET dead_ct = dead_ct + @interval WHERE session_id = ANY(@sessionIds)";

    public string UpdatePlayedSpec =>
        "UPDATE played SET spec = spec + @interval WHERE session_id = ANY(@sessionIds)";
}
