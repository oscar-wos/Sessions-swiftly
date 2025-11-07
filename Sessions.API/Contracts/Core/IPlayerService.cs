using Sessions.API.Structs;

namespace Sessions.API.Contracts.Core;

public interface IPlayerService
{
    Player? Player { get; }
    Session? Session { get; }
}
