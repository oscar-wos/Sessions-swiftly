using RSession.Shared.Contracts;

namespace RSession.Contracts.Core;

internal interface IRSessionServerInternal : IRSessionServer
{
    void Initialize();
}
