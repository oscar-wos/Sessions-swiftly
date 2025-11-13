using SwiftlyS2.Shared.Players;

namespace RSession.API.Delegates;

public delegate void OnPlayerRegisteredDelegate(IPlayer player, int playerId, long sessionId);
