namespace Sessions.API.Structs;

public readonly struct Player
{
    public readonly required int Id { get; init; }
    public readonly Session? Session { get; init; }
}
