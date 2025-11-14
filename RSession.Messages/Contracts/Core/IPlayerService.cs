using RSession.Shared.Contracts;
using SwiftlyS2.Shared.Players;

namespace RSession.Messages.Contracts.Core;

internal interface IPlayerService
{
    void Initialize(IRSessionPlayer sessionPlayer, IRSessionServer sessionServer);
    void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message);
}
