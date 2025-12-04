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
using RSession.Rotation.Contracts.Core;
using RSession.Rotation.Contracts.Database;
using RSession.Rotation.Contracts.Log;
using RSession.Shared.Contracts.Core;

namespace RSession.Rotation.Services.Core;

internal sealed class MapService(
    ILogService logService,
    ILogger<MapService> logger,
    IDatabaseFactory databaseFactory
) : IMapService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<MapService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;
    private ISessionServerService? _sessionServerService;

    public void Initialize(ISessionServerService sessionServerService) =>
        _sessionServerService = sessionServerService;

    public void HandleMapRegistered(short mapId) =>
        Task.Run(async () =>
        {
            if (_databaseFactory.GetDatabaseService() is not { } databaseService)
            {
                _logService.LogWarning("Database service not available", logger: _logger);
                return;
            }

            if (_sessionServerService?.GetServerId() is not { } serverId)
            {
                _logService.LogWarning($"Server not registered", logger: _logger);
                return;
            }

            try
            {
                await databaseService.InsertRotationAsync(serverId, mapId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to insert rotation - {mapId}",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
