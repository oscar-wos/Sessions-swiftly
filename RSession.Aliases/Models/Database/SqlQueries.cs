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
using RSession.Aliases.Contracts.Database;

namespace RSession.Aliases.Models.Database;

internal sealed class SqlQueries : LoadQueries, IDatabaseQueries
{
    protected override string CreateAliases =>
        """
            CREATE TABLE IF NOT EXISTS aliases (
                id BIGINT AUTO_INCREMENT PRIMARY KEY,
                player_id INT NOT NULL,
                alias VARCHAR(128) NOT NULL,
                timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;

    public string SelectAlias =>
        "SELECT alias FROM aliases WHERE player_id = @playerId ORDER BY id DESC";

    public string InsertAlias =>
        "INSERT INTO aliases (player_id, alias) VALUES (@playerId, @alias)";
}
