using Sessions.API.Structs;

namespace Sessions.API.Contracts.Core;

public interface IServerService
{
    Server? Server { get; }
    Map? Map { get; }
}
