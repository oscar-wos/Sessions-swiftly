// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.Logging;
using RSession.Played.Contracts.Event;
using RSession.Played.Contracts.Log;
using RSession.Shared.Contracts.Core;

namespace RSession.Played.Services.Event;

internal sealed class OnElapsedService(ILogService logService, ILogger<OnElapsedService> logger)
    : IOnElapsedService,
        IDisposable
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnElapsedService> _logger = logger;

    private ISessionEventService? _sessionEventService;

    public void Initialize(ISessionEventService sessionEventService)
    {
        _sessionEventService = sessionEventService;

        _sessionEventService.OnElapsed += OnElapsed;
        _sessionEventService.OnDispose += OnDispose;

        _logService.LogInformation("OnElapsed subscribed", logger: _logger);
    }

    private void OnElapsed() { }

    private void OnDispose() => Dispose();

    public void Dispose() => _sessionEventService?.OnElapsed -= OnElapsed;
}
