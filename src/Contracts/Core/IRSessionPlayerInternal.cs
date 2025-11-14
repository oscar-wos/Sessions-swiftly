using RSession.Shared.Contracts.Core;
using SwiftlyS2.Shared.Players;

namespace RSession.Contracts.Core;

internal interface IRSessionPlayerInternal : IRSessionPlayer
{
    void HandlePlayerAuthorize(IPlayer player, short serverId);
    void HandlePlayerDisconnected(IPlayer player);
}
