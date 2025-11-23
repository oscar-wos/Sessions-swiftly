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
using RSession.Messages.Contracts.Core;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Log;
using RSession.Shared.Contracts.Core;
using SwiftlyS2.Shared.Players;

namespace RSession.Messages.Services.Core;

internal sealed class PlayerService(
    ILogService logService,
    ILogger<PlayerService> logger,
    IDatabaseFactory databaseFactory
) : IPlayerService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;
    private ISessionPlayerService? _sessionPlayerService;

    public void Initialize(ISessionPlayerService sessionPlayerService) =>
        _sessionPlayerService = sessionPlayerService;

    public void HandlePlayerMessage(IPlayer player, short teamNum, bool teamChat, string message) =>
        Task.Run(async () =>
        {
            if (_databaseFactory.GetDatabaseService() is not { } databaseService)
            {
                _logService.LogWarning("Database service not available", logger: _logger);
                return;
            }

            if (_sessionPlayerService?.GetSessionId(player) is not { } sessionId)
            {
                _logService.LogWarning(
                    $"Player not registered - {player.Controller.PlayerName} ({player.SteamID})",
                    logger: _logger
                );

                return;
            }

            try
            {
                await databaseService
                    .InsertMessageAsync(sessionId, teamNum, teamChat, message)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    $"Unable to insert message - {player.Controller.PlayerName} ({player.SteamID}) : {message}",
                    exception: ex,
                    logger: _logger
                );
            }
        });
}
