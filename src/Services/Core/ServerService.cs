// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
    IEventService eventService
) : IServerService
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<ServerService> _logger = logger;

    private readonly IDatabaseService _databaseService = databaseFactory.GetDatabaseService();
    private readonly IEventService _eventService = eventService;

    private short? _id;

    public short? GetServerId() => _id;

    public void Initialize() =>
        Task.Run(async () =>
        {
            string ip = _core.Engine.ServerIP ?? "0.0.0.0";
            ushort port = (ushort)(_core.ConVar.Find<int>("hostport")?.Value ?? 0);

            try
            {
                await _databaseService.CreateTablesAsync().ConfigureAwait(false);

                short serverId = await _databaseService
                    .GetServerAsync(ip, port)
                    .ConfigureAwait(false);

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
