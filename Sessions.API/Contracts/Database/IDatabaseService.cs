using Sessions.API.Structs;
using SwiftlyS2.Shared.Players;

namespace Sessions.API.Contracts.Database;

public interface IDatabaseService
{
    Task StartAsync();
    Task<Alias> GetAliasAsync(int playerId);
    Task<Map> GetMapAsync(string mapName);
    Task<Player> GetPlayerAsync(ulong steamId);
    Task<Server> GetServerAsync(string serverIp, ushort serverPort);
    Task<Session> GetSessionAsync(int playerId, int serverId, int mapId, string ip);
    Task InsertAliasAsync(long sessionId, int playerId, string name);
    Task InsertMessageAsync(long sessionId, int playerId, MessageType messageType, string message);
    Task UpdateSeenAsync(int playerId);
    Task UpdateSessionsAsync(IEnumerable<int> playerIds, IEnumerable<long> sessionIds);
}
