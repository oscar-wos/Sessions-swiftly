using RSession.Shared.Contracts;

namespace RSession.Contracts.Database;

internal interface IDatabaseService : ISessionDatabaseService
{
    Task InitializeAsync();
    Task<int> GetPlayerAsync(ulong steamId);
    Task<short> GetServerAsync(string serverIp, ushort serverPort);
    Task<long> GetSessionAsync(int playerId, short serverId, string ip);
    Task UpdateSessionsAsync(List<int> playerIds, List<long> sessionIds);
}
