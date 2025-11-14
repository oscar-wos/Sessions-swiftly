namespace RSession.Contracts.Database;

internal interface IDatabaseFactory
{
    IDatabaseService Database { get; }
}
