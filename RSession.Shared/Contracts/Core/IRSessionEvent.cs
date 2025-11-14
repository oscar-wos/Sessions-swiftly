using RSession.Shared.Delegates;

namespace RSession.Shared.Contracts.Core;

public interface IRSessionEvent
{
    event OnPlayerRegisteredDelegate OnPlayerRegistered;
    event OnServerRegisteredDelegate OnServerRegistered;
}
