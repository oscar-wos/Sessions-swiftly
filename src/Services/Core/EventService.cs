using RSession.API.Contracts.Core;
using RSession.API.Delegates;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

public sealed class EventService : IEventService
{
    public event OnPlayerRegisteredDelegate? OnPlayerRegistered;
    public event OnServerRegisteredDelegate? OnServerRegistered;

    public void InvokePlayerRegistered(IPlayer player, int playerId, long sessionId) =>
        OnPlayerRegistered?.Invoke(player, playerId, sessionId);

    public void InvokeServerRegistered(short serverId) => OnServerRegistered?.Invoke(serverId);
}
