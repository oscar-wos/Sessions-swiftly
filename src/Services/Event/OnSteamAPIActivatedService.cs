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

namespace RSession.Services.Event;

internal sealed class OnSteamAPIActivatedService(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnSteamAPIActivatedService> logger,
    IServerService serverService
) : IEventListener, IDisposable
{
    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnSteamAPIActivatedService> _logger = logger;

    private readonly IServerService _serverService = serverService;

    public void Subscribe()
    {
        _core.Event.OnSteamAPIActivated += OnSteamAPIActivated;
        _logService.LogInformation("OnSteamAPIActivated subscribed", logger: _logger);
    }

    private void OnSteamAPIActivated()
    {
        _logService.LogDebug($"SteamAPI activated", logger: _logger);
        _serverService.Initialize();
    }

    public void Dispose() => _core.Event.OnSteamAPIActivated -= OnSteamAPIActivated;
}
