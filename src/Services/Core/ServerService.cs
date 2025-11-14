using Microsoft.Extensions.Logging;
using RSession.Contracts.Core;
using RSession.Contracts.Database;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;

namespace RSession.Services.Core;

internal sealed class ServerService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<ServerService> logger,
    IDatabaseFactory databaseFactory,
    IRSessionEventInternal eventService
) : IRSessionServerInternal
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<ServerService> _logger = logger;

    private readonly IDatabaseService _database = databaseFactory.Database;
    private readonly IRSessionEventInternal _eventService = eventService;

    private short? _id;

    public short? GetServerId() => _id;

    public void Initialize() =>
        Task.Run(async () =>
        {
            string ip = _core.Engine.ServerIP ?? "0.0.0.0";
            ushort port = (ushort)(_core.ConVar.Find<int>("hostport")?.Value ?? 0);

            try
            {
                await _database.InitAsync().ConfigureAwait(false);
                short serverId = await _database.GetServerAsync(ip, port).ConfigureAwait(false);

                _logService.LogInformation(
                    $"Server registered - {ip}:{port} | Server ID: {serverId}",
                    logger: _logger
                );

                _id = serverId;

                _eventService.InvokeServerRegistered(serverId);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to register server - {ip}:{port}",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
