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
using RSession.Aliases.Contracts.Database;
using RSession.Aliases.Contracts.Event;
using RSession.Aliases.Contracts.Log;
using RSession.Shared.Contracts.Core;
using RSession.Shared.Contracts.Database;

namespace RSession.Aliases.Services.Event;

internal sealed class OnDatabaseConfiguredService(
    ILogService logService,
    ILogger<OnDatabaseConfiguredService> logger,
    IDatabaseFactory databaseFactory
) : IOnDatabaseConfiguredService, IDisposable
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnDatabaseConfiguredService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;
    private ISessionEventService? _sessionEventService;

    public void Initialize(ISessionEventService sessionEventService)
    {
        _sessionEventService = sessionEventService;

        _sessionEventService.OnDatabaseConfigured += OnDatabaseConfigured;
        _sessionEventService.OnDispose += OnDispose;
        _logService.LogInformation("OnDatabaseConfigured subscribed", logger: _logger);
    }

    private void OnDatabaseConfigured(ISessionDatabaseService databaseService, string type, string prefix) =>
        _databaseFactory.Initialize(databaseService, type, prefix);

    private void OnDispose() => Dispose();

    public void Dispose() => _sessionEventService?.OnDatabaseConfigured -= OnDatabaseConfigured;
}
