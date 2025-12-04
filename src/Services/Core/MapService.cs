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

internal sealed class MapService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<MapService> logger,
    IDatabaseFactory databaseFactory,
    IEventService eventService
) : IMapService
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<MapService> _logger = logger;

    private readonly IDatabaseService _databaseService = databaseFactory.GetDatabaseService();
    private readonly IEventService _eventService = eventService;

    private string? _lastMapName;
    private short? _id;

    public short? GetMapId() => _id;

    public void HandleMapLoad(string mapName) =>
        _core.Scheduler.NextWorldUpdate(() => OnMapLoad(mapName));

    private void OnMapLoad(string mapName) =>
        Task.Run(async () =>
        {
            if (_lastMapName == mapName)
            {
                return;
            }

            _lastMapName = mapName;

            string workshopIdString = _core.Engine.WorkshopId;

            long? workshopId = string.IsNullOrEmpty(workshopIdString)
                ? null
                : long.Parse(workshopIdString);

            try
            {
                short mapId = await _databaseService
                    .GetMapAsync(mapName, workshopId)
                    .ConfigureAwait(false);

                _logService.LogInformation(
                    $"Map registered - {mapName} ({workshopId}) | Map ID: {mapId}",
                    logger: _logger
                );

                _id = mapId;
                _eventService.InvokeMapRegistered(mapId);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to register map - {mapName} ({workshopId})",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
