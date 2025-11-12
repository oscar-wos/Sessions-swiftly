using Microsoft.Extensions.Logging;
using RSession.API.Contracts.Core;
using RSession.API.Contracts.Database;
using RSession.API.Contracts.Log;
using SwiftlyS2.Shared;

namespace RSession.Services.Core;

public sealed class ServerService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<ServerService> logger,
    IDatabaseFactory databaseFactory,
    IPlayerService playerService
) : IServerService
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<ServerService> _logger = logger;

    private readonly IDatabaseService _database = databaseFactory.Database;
    private readonly IPlayerService _playerService = playerService;

    public short? Id { get; private set; }

    public void Init() =>
        Task.Run(async () =>
        {
            string ip = _core.Engine.ServerIP ?? "0.0.0.0";
            ushort port = (ushort)(_core.ConVar.Find<int>("hostport")?.Value ?? 0);

            try
            {
                await _database.InitAsync().ConfigureAwait(false);
                short serverId = await _database.GetServerAsync(ip, port);

                _logService.LogInformation(
                    $"Server initialized - {ip}:{port} | Server ID: {serverId}",
                    logger: _logger
                );

                Id = serverId;

                _playerService.Init(serverId);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to initialize server - {ip}:{port}",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
