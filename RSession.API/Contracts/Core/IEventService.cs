using RSession.API.Delegates;
using SwiftlyS2.Shared.Players;

namespace RSession.API.Contracts.Core;

public interface IEventService
{
    event OnPlayerRegisteredDelegate OnPlayerRegistered;
    event OnServerRegisteredDelegate OnServerRegistered;
    void InvokePlayerRegistered(IPlayer player, int playerId, long sessionId);
    void InvokeServerRegistered(short serverId);
}
