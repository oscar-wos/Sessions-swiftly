using RSession.Shared.Contracts;

namespace RSession.Maps.Contracts.Event;

internal interface IOnDatabaseConfiguredService
{
    void Initialize(ISessionEventService sessionEventService);
}
