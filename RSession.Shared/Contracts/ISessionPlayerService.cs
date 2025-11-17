using RSession.Shared.Structs;
using SwiftlyS2.Shared.Players;

namespace RSession.Shared.Contracts;

public interface ISessionPlayerService
{
    SessionPlayer? GetSessionPlayer(IPlayer player);
    int? GetPlayerId(IPlayer player);
    long? GetSessionId(IPlayer player);
}
