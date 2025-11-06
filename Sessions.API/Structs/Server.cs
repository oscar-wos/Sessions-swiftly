namespace Sessions.API.Structs;

public readonly struct Server
{
    public readonly required string Ip { get; init; }
    public readonly required short Port { get; init; }
    public readonly required short Id { get; init; }
    public readonly Map? Map { get; init; }
}
