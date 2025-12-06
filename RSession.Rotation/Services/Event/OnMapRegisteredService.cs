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
using RSession.Rotation.Contracts.Event;
using RSession.Rotation.Contracts.Log;
using RSession.Shared.Contracts.Core;

namespace RSession.Rotation.Services.Event;

internal class OnMapRegisteredService(
    ILogService logService,
    ILogger<OnMapRegisteredService> logger,
    IMapService mapService
) : IOnMapRegisteredService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnMapRegisteredService> _logger = logger;

    private readonly IMapService _mapService = mapService;
    private ISessionEventService? _sessionEventService;

    public void Initialize(ISessionEventService sessionEventService)
    {
        _sessionEventService = sessionEventService;

        _sessionEventService.OnMapRegistered += OnMapRegistered;
        _sessionEventService.OnDispose += OnDispose;

        _logService.LogInformation("OnMapRegistered subscribed", logger: _logger);
    }

    private void OnMapRegistered(short mapId)
    {
        _logService.LogDebug($"Rotation - {mapId}", logger: _logger);
        _mapService.HandleMapRegistered(mapId);
    }

    private void OnDispose() => Dispose();

    public void Dispose() => _sessionEventService?.OnMapRegistered -= OnMapRegistered;
}
