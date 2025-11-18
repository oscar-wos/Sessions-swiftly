namespace RSession.Maps.Models.Database;

internal abstract class LoadQueries
{
    protected abstract string CreateMaps { get; }
    protected abstract string CreateRotations { get; }

    public IEnumerable<string> GetLoadQueries()
    {
        yield return CreateMaps;
        yield return CreateRotations;
    }
}
