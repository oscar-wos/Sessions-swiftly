namespace RSession.API.Contracts.Core;

public interface IServerService
{
    short? Id { get; }
    void Init();
}
