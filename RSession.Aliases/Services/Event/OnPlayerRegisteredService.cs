// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
using Microsoft.Extensions.Logging;
using RSession.Aliases.Contracts.Database;
using RSession.Aliases.Contracts.Event;
using RSession.Aliases.Contracts.Log;
using RSession.Shared.Contracts;
using RSession.Shared.Structs;
using SwiftlyS2.Shared.Players;

namespace RSession.Aliases.Services.Event;

internal class OnPlayerRegisteredService(
    ILogService logService,
    ILogger<OnPlayerRegisteredService> logger,
    IDatabaseFactory databaseFactory
) : IOnPlayerRegisteredService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnPlayerRegisteredService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;

    private ISessionEventService? _sessionEventService;

    public void Initialize(ISessionEventService sessionEventService)
    {
        _sessionEventService = sessionEventService;

        _sessionEventService.OnPlayerRegistered += OnPlayerRegistered;
        _logService.LogInformation("OnPlayerRegistered subscribed", logger: _logger);
    }

    private void OnPlayerRegistered(IPlayer player, in SessionPlayer sessionPlayer) { }

    public void Dispose()
    {
        _sessionEventService?.OnPlayerRegistered -= OnPlayerRegistered;
        _logService.LogInformation("OnPlayerRegisteredService disposed", logger: _logger);
    }
}
