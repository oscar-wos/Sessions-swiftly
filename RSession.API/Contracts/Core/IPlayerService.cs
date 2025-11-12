using SwiftlyS2.Shared.Players;

namespace RSession.API.Contracts.Core;

public interface IPlayerService
{
    int? GetPlayer(IPlayer player);
    long? GetSession(IPlayer player);
    void Init(short serverId);
    void HandlePlayerAuthorize(IPlayer player, short serverId);
    void HandlePlayerDisconnected(IPlayer player);
}
