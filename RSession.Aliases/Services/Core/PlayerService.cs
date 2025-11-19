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
using RSession.Aliases.Contracts.Core;
using RSession.Aliases.Contracts.Database;
using RSession.Aliases.Contracts.Log;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;

namespace RSession.Aliases.Services.Core;

internal sealed class PlayerService(
    ILogService logService,
    ILogger<PlayerService> logger,
    IDatabaseFactory databaseFactory
) : IPlayerService
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;

    public void HandlePlayerAlias(IPlayer player, int playerId) =>
        Task.Run(async () =>
        {
            string playerName = player.Controller.PlayerName;
            uint playerNameHash = MurmurHash2.HashString(playerName);

            if (_databaseFactory.GetDatabaseService() is { } databaseService)
            {
                try
                {
                    if (
                        await databaseService.SelectAliasAsync(playerId).ConfigureAwait(false)
                        is not { } checkAlias
                    )
                    {
                        return;
                    }

                    uint checkAliasHash = MurmurHash2.HashString(checkAlias);

                    if (playerNameHash == checkAliasHash)
                    {
                        return;
                    }

                    await databaseService
                        .InsertAliasAsync(playerId, playerName)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logService.LogError(
                        $"Unable to insert alias - {playerName} ({player.SteamID})",
                        exception: ex,
                        logger: _logger
                    );
                }
            }
        });
}
