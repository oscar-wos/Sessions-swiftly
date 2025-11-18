using RSession.Shared.Contracts;
using SwiftlyS2.Shared.Players;

namespace RSession.Maps.Contracts.Core;

internal interface IPlayerService
{
    void Initialize(ISessionPlayerService sessionPlayerService);
    void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message);
}
