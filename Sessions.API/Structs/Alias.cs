namespace Sessions.API.Structs;

public readonly struct Alias
{
    public readonly required int Id { get; init; }
    public readonly required string Name { get; init; }
}
