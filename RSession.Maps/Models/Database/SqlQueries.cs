using RSession.Maps.Contracts.Database;

namespace RSession.Maps.Models.Database;

internal sealed class SqlQueries : LoadQueries, IDatabaseQueries
{
    protected override string CreateMaps =>
        """
            CREATE TABLE IF NOT EXISTS messages (
                id BIGINT AUTO_INCREMENT PRIMARY KEY,
                session_id BIGINT NOT NULL,
                timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                team_num SMALLINT NOT NULL,
                team_chat BOOLEAN NOT NULL,
                message VARCHAR(512) COLLATE utf8mb4_unicode_520_ci
            )
            """;

    public string InsertMap =>
        "INSERT INTO messages (session_id, team_num, team_chat, message) VALUES (@sessionId, @teamNum, @teamChat, @message)";
}
