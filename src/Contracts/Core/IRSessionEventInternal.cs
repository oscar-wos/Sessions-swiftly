using RSession.Shared.Contracts.Core;
using SwiftlyS2.Shared.Players;

namespace RSession.Contracts.Core;

internal interface IRSessionEventInternal : IRSessionEvent
{
    void InvokePlayerRegistered(IPlayer player, int playerId, long sessionId);
    void InvokeServerRegistered(short serverId);
}
