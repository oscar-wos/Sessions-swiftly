using RSession.Messages.Contracts.Database;

namespace RSession.Messages.Models.Database;

internal sealed class PostgresQueries : LoadQueries, IDatabaseQueries
{
    protected override string CreateMessages =>
        """
            CREATE TABLE IF NOT EXISTS messages (
                id BIGSERIAL PRIMARY KEY,
                session_id BIGINT NOT NULL,    
                timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                team_num SMALLINT NOT NULL,
                team_chat BOOLEAN NOT NULL,
                message VARCHAR(512)
            )
            """;

    public string InsertMessage =>
        "INSERT INTO messages (session_id, team_num, team_chat, message) VALUES (@sessionId, @teamNum, @teamChat, @message)";
}
