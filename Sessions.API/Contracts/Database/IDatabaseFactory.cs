namespace Sessions.API.Contracts.Database;

public interface IDatabaseFactory
{
    IDatabaseService Database { get; }
}
