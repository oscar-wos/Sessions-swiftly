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
using RSession.Messages.Contracts.Hook;
using RSession.Messages.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace RSession.Messages.Services.Hook;

internal sealed class OnUserMessageSayText2Service(
    ISwiftlyCore core,
    ILogService logService,
    ILogger<OnUserMessageSayText2Service> logger,
    IPlayerService playerService
) : IHook, IDisposable
{
    private static readonly uint _cStrikeChatAllHash = MurmurHash2.HashString("Cstrike_Chat_All");

    private static readonly uint _cStrikeChatAllSpecHash = MurmurHash2.HashString(
        "Cstrike_Chat_AllSpec"
    );

    private readonly ISwiftlyCore _core = core;
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnUserMessageSayText2Service> _logger = logger;

    private readonly IPlayerService _playerService = playerService;

    private Guid _cUserMessageSayText2Guid;

    public void Register()
    {
        _cUserMessageSayText2Guid = _core.NetMessage.HookServerMessage<CUserMessageSayText2>(msg =>
        {
            OnUserMessageSayText2(in msg);
            return HookResult.Continue;
        });

        if (_cUserMessageSayText2Guid == Guid.Empty)
        {
            _logService.LogWarning("CUserMessageSayText2 not hooked", logger: _logger);
        }
        else
        {
            _logService.LogInformation(
                $"CUserMessageSayText2 hooked - {_cUserMessageSayText2Guid}",
                logger: _logger
            );
        }
    }

    private void OnUserMessageSayText2(in CUserMessageSayText2 msg)
    {
        int playerId = msg.Entityindex - 1;

        if (_core.PlayerManager.GetPlayer(playerId) is not { } player)
        {
            _logService.LogWarning(
                $"OnUserMessageSayText2 Player not found - {playerId}",
                logger: _logger
            );

            return;
        }

        string message = msg.Param2;
        string messageName = msg.Messagename;

        short teamNum = player.Controller.TeamNum;
        bool teamChat = true;

        uint messageNameHash = MurmurHash2.HashString(messageName);

        if (messageNameHash == _cStrikeChatAllHash || messageNameHash == _cStrikeChatAllSpecHash)
        {
            teamChat = false;
        }

        _logService.LogDebug(
            $"Message - {player.Controller.PlayerName} ({player.SteamID}): {message} ({messageName})",
            logger: _logger
        );

        _playerService.HandlePlayerMessage(player, teamNum, teamChat, message);
    }

    public void Dispose()
    {
        if (_cUserMessageSayText2Guid == Guid.Empty)
        {
            return;
        }

        _core.NetMessage.Unhook(_cUserMessageSayText2Guid);
    }
}
