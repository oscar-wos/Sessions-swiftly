using Microsoft.Extensions.Logging;
using Sessions.API.Contracts.Database;
using Sessions.API.Contracts.Hook;
using Sessions.API.Contracts.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Sessions.Services.Hook;

public sealed class PlayerMessageService(
    ISwiftlyCore core,
    IDatabaseFactory databaseFactory,
    ILogService logService,
    ILogger<PlayerMessageService> logger
) : IPlayerMessageService
{
    private static readonly uint _cStrikeChatAllHash = MurmurHash2.HashString("Cstrike_Chat_All");

    private readonly ISwiftlyCore _core = core;
    private readonly IDatabaseService _database = databaseFactory.Database;

    private readonly ILogService _logService = logService;
    private readonly ILogger<PlayerMessageService> _logger = logger;

    public void OnClientMessage(in CUserMessageSayText2 msg)
    {
        int playerId = msg.Entityindex - 1;

        if (_core.PlayerManager.GetPlayer(playerId) is not { } player)
        {
            _logService.LogWarning($"Player not found - {playerId}", logger: _logger);
            return;
        }

        string message = msg.Param2;
        string messageName = msg.Messagename;

        _logService.LogDebug(
            $"Message - {player.Controller.PlayerName}: {message} ({messageName})",
            logger: _logger
        );

        uint hash = MurmurHash2.HashString(messageName);

        short teamNum = player.Controller.TeamNum;
        bool teamChat = true;

        if (hash == _cStrikeChatAllHash)
        {
            teamChat = false;
        }

        _ = Task.Run(async () =>
            await _database.InsertMessageAsync(0, 0, teamNum, teamChat, message)
        );
    }
}
