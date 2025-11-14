using RSession.Messages.Contracts.Core;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared.Players;

namespace RSession.Messages.Services.Core;

internal sealed class PlayerService : IPlayerService
{
    private IRSessionPlayer? _sessionPlayer;
    private IRSessionServer? _sessionServer;

    public void Initialize(IRSessionPlayer sessionPlayer, IRSessionServer sessionServer)
    {
        _sessionPlayer = sessionPlayer;
        _sessionServer = sessionServer;
    }

    public void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message)
    {
        if (
            _sessionPlayer?.GetPlayerId(player) is not { } playerId
            || _sessionPlayer?.GetSessionId(player) is not { } sessionId
            || _sessionServer?.GetServerId() is not { } serverId
        )
        {
            return;
        }
    }
}
