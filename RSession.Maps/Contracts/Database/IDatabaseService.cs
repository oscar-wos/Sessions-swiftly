using RSession.Shared.Contracts;

namespace RSession.Maps.Contracts.Database;

internal interface IDatabaseService
{
    void Initialize(ISessionDatabaseService databaseService);
    Task CreateTablesAsync();
    Task InsertMessageAsync(long sessionId, short teamNum, bool teamChat, string message);
}
