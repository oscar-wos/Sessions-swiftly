using RSession.Shared.Contracts;

namespace RSession.Messages.Contracts.Event;

internal interface IOnDatabaseConfiguredService
{
    void Initialize(ISessionEventService sessionEventService);
}
