using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Log;
using Sessions.API.Models.Config;
using Sessions.API.Structs;
using SwiftlyS2.Shared.Players;

namespace Sessions.Services.Database;

public sealed class PostgresService : IPostgresService, IDatabaseService, IDisposable
{
    private readonly IOptionsMonitor<DatabaseConfig> _config;
    private readonly ILogService _logService;
    private readonly ILogger<PostgresService> _logger;
    private readonly NpgsqlConnection _connection;

    public PostgresService(
        IOptionsMonitor<DatabaseConfig> config,
        ILogService logService,
        ILogger<PostgresService> logger
    )
    {
        _config = config;
        _logService = logService;
        _logger = logger;

        _connection = new NpgsqlConnection(_config.CurrentValue.Connection.Host);
    }

    public async Task StartAsync()
    {
        try { }
        catch (NpgsqlException ex)
        {
            _logService.LogError("PostgresService.StartAsync()", ex, logger: _logger);
            throw;
        }
    }

    public Task<Alias> GetAliasAsync(int playerId) => throw new NotImplementedException();

    public Task<Map> GetMapAsync(string mapName) => throw new NotImplementedException();

    public Task<Player> GetPlayerAsync(ulong steamId) => throw new NotImplementedException();

    public Task<Server> GetServerAsync(string serverIp, ushort serverPort) =>
        throw new NotImplementedException();

    public Task<Session> GetSessionAsync(int playerId, int serverId, int mapId, string ip) =>
        throw new NotImplementedException();

    public Task InsertAliasAsync(long sessionId, int playerId, string name) =>
        throw new NotImplementedException();

    public Task InsertMessageAsync(
        long sessionId,
        int playerId,
        MessageType messageType,
        string message
    ) => throw new NotImplementedException();

    public Task UpdateSeenAsync(int playerId) => throw new NotImplementedException();

    public Task UpdateSessionsAsync(IEnumerable<int> playerIds, IEnumerable<long> sessionIds) =>
        throw new NotImplementedException();

    public void Dispose() { }
}
