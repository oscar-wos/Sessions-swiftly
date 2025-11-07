namespace Sessions.API.Models.Config;

public sealed class ConnectionConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 0;
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
