namespace Sessions.API.Models.Config;

public sealed class DatabaseConfig
{
    public string Type { get; set; } = "";
    public ConnectionConfig Connection { get; set; } = new();
}
