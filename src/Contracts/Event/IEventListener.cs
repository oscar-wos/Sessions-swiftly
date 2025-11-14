namespace RSession.Contracts.Event;

internal interface IEventListener
{
    void Subscribe();
    void Unsubscribe();
}
