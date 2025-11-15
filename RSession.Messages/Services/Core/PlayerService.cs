using RSession.Messages.Contracts.Core;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared.Players;

namespace RSession.Messages.Services.Core;

internal sealed class PlayerService : IPlayerService
{
    private IRSessionPlayerService? _sessionPlayerService;
    private IRSessionServerService? _sessionServerService;

    public void Initialize(
        IRSessionPlayerService sessionPlayerService,
        IRSessionServerService sessionServerService
    )
    {
        _sessionPlayerService = sessionPlayerService;
        _sessionServerService = sessionServerService;
    }

    public void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message)
    {
        if (
            _sessionPlayerService?.GetPlayerId(player) is not { } playerId
            || _sessionPlayerService?.GetSessionId(player) is not { } sessionId
            || _sessionServerService?.GetServerId() is not { } serverId
        )
        {
            return;
        }
    }
}
