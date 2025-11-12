using RSession.API.Contracts.Core;
using RSession.API.Delegates;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

public sealed class EventService : IEventService
{
    public event PlayerAuthorizedDelegate? PlayerAuthorized;
    public event ServerAuthorizedDelegate? ServerAuthorized;

    public void InvokePlayerAuthorized(IPlayer player, int playerId, long sessionId) =>
        PlayerAuthorized?.Invoke(player, playerId, sessionId);

    public void InvokeServerAuthorized(short serverId) => ServerAuthorized?.Invoke(serverId);
}
