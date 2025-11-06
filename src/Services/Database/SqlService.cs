using Sessions.API.Contracts.Database;
using Sessions.API.Structs;
using SwiftlyS2.Shared.Players;

namespace Sessions.Services.Database;

internal class SqlService : IDatabaseService, ISqlService
{
    public Task StartAsync() => throw new NotImplementedException();

    public Task<Alias> GetAliasAsync(int playerId) => throw new NotImplementedException();

    public Task<Map> GetMapAsync(string mapName) => throw new NotImplementedException();

    public Task<Player> GetPlayerAsync(ulong steamId) => throw new NotImplementedException();

    public Task<Server> GetServerAsync(string serverIp, ushort serverPort) =>
        throw new NotImplementedException();

    public Task<Session> GetSessionAsync(int playerId, int serverId, int mapId, string ip) =>
        throw new NotImplementedException();

    public Task InsertAliasAsync(long sessionId, int playerId, string name) =>
        throw new NotImplementedException();

    public Task InsertMessageAsync(
        long sessionId,
        int playerId,
        MessageType messageType,
        string message
    ) => throw new NotImplementedException();

    public Task UpdateSeenAsync(int playerId) => throw new NotImplementedException();

    public Task UpdateSessionsAsync(IEnumerable<int> playerIds, IEnumerable<long> sessionIds) =>
        throw new NotImplementedException();
}
