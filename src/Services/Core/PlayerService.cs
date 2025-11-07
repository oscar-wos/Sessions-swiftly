using Sessions.API.Contracts.Core;
using Sessions.API.Structs;

namespace Sessions.Services.Core;

internal class PlayerService : IPlayerService
{
    public Player? Player => throw new NotImplementedException();

    public Session? Session => throw new NotImplementedException();
}
