namespace RSession.Contracts.Database;

internal interface IDatabaseQueries
{
    string SelectPlayer { get; }
    string InsertPlayer { get; }

    string SelectServer { get; }
    string InsertServer { get; }

    string InsertSession { get; }

    string UpdateSeen { get; }
    string UpdateSession { get; }
}
