using RSession.Maps.Contracts.Database;

namespace RSession.Maps.Models.Database;

internal sealed class PostgresQueries : LoadQueries, IDatabaseQueries
{
    protected override string CreateMaps =>
        """
            CREATE TABLE IF NOT EXISTS maps (
                id SMALLSERIAL PRIMARY KEY,
                name VARCHAR(64) NOT NULL,
                workshop_id BIGINT
            )
            """;

    protected override string CreateRotations =>
        """
            CREATE TABLE IF NOT EXISTS rotations (
                id SERIAL PRIMARY KEY,
                server_id SMALLINT NOT NULL,
                map_id SMALLINT NOT NULL,   
                timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;

    public string InsertMap => "INSERT INTO maps (name) VALUES (@name)";
    public string InsertRotation =>
        "INSERT INTO rotations (server_id, map_id) VALUES (@serverId, @mapId)";
}
