using BepInEx.Logging;
using GameNetcodeStuff;
using System;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class KillCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public KillCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        var player = GameNetworkManager.Instance.localPlayerController;
        CommandTitle = "Kill";
        CommandBody = $"Killed {player.playerUsername}.";
        player.KillPlayer(player.transform.position, true);
        logger.LogInfo($"Killed {player.playerUsername}");
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "KillError Error";
        CommandBody = $"Error Killing Player";
    }
}

