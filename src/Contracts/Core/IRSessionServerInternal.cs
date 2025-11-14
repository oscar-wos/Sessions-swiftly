using RSession.Shared.Contracts.Core;

namespace RSession.Contracts.Core;

internal interface IRSessionServerInternal : Shared.Contracts.Core.IRSessionServer
{
    void Init();
}
