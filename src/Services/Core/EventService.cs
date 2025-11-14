using RSession.Contracts.Core;
using RSession.Shared.Delegates;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

internal sealed class EventService : IRSessionEventInternal
{
    public event OnPlayerRegisteredDelegate? OnPlayerRegistered;
    public event OnServerRegisteredDelegate? OnServerRegistered;

    public void InvokePlayerRegistered(IPlayer player, int playerId, long sessionId) =>
        OnPlayerRegistered?.Invoke(player, playerId, sessionId);

    public void InvokeServerRegistered(short serverId) => OnServerRegistered?.Invoke(serverId);
}
