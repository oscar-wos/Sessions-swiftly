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
using Microsoft.Extensions.Options;
using RSession.Contracts.Core;
using RSession.Contracts.Log;
using RSession.Contracts.Schedule;
using RSession.Models.Config;
using Timer = System.Timers.Timer;

namespace RSession.Services.Schedule;

internal sealed class IntervalService(
    ILogService logService,
    ILogger<IntervalService> logger,
    IOptionsMonitor<SessionConfig> config,
    IEventService eventService
) : IIntervalService, IDisposable
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<IntervalService> _logger = logger;
    private readonly IOptionsMonitor<SessionConfig> _config = config;

    private readonly IEventService _eventService = eventService;

    private Timer? _timer;

    public void Initialize()
    {
        _timer = new Timer(_config.CurrentValue.UpdateIntervalSeconds * 1000)
        {
            AutoReset = true,
            Enabled = true,
        };

        _timer.Elapsed += async (_, _) => await OnElapsed().ConfigureAwait(false);
        _logService.LogInformation("IntervalService initialized", logger: _logger);
    }

    private async Task OnElapsed() =>
        _eventService.InvokeElapsed(_config.CurrentValue.UpdateIntervalSeconds);

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
