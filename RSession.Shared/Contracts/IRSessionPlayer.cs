using SwiftlyS2.Shared.Players;

namespace RSession.Shared.Contracts;

public interface IRSessionPlayer
{
    int? GetPlayerId(IPlayer player);
    long? GetSessionId(IPlayer player);
}
