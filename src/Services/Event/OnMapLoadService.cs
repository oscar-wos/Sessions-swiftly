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
using RSession.Contracts.Event;
using RSession.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;

namespace RSession.Services.Event;

internal sealed class OnMapLoadService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnMapLoadService> logger,
    IMapService mapService
) : IEventListener, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnMapLoadService> _logger = logger;

    private readonly IMapService _mapService = mapService;

    public void Subscribe()
    {
        _core.Event.OnMapLoad += OnMapLoad;
        _logService.LogInformation("OnMapLoad subscribed", logger: _logger);
    }

    private void OnMapLoad(IOnMapLoadEvent @event)
    {
        string mapName = @event.MapName;

        _logService.LogDebug($"Map loaded {mapName}", logger: _logger);
        _mapService.HandleMapLoad(mapName);
    }

    public void Dispose() => _core.Event.OnMapLoad -= OnMapLoad;
}
