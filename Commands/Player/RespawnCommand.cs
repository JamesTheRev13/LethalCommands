using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class RespawnCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public RespawnCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Respawn";
        var player = GameNetworkManager.Instance.localPlayerController;
        var spectatedPlayer = player.spectatedPlayerScript;

        if (player.isPlayerDead)
        {
            player.isPlayerDead = false;
            player.isPlayerControlled = true;
            player.thisPlayerModelArms.enabled = true;
            player.localVisor.position = spectatedPlayer.transform.position;
            player.DisablePlayerModel(player.gameObject, true, false);
            player.isInsideFactory = spectatedPlayer.isInsideFactory;
            player.IsInspectingItem = false;
            player.inTerminalMenu = false;
            player.setPositionOfDeadPlayer = false;
            // ???
            player.snapToServerPosition = true;
            HUDManager.Instance.RemoveSpectateUI();
            HUDManager.Instance.HideHUD(hide: false);
            StartOfRound.Instance.livingPlayers++;
            StartOfRound.Instance.allPlayersDead = false;
            CommandBody = $"Respawned {player.playerUsername}";
            return;
        }
        CommandBody = $"{player.playerUsername} is not dead.";
        logger.LogInfo($"Failed to respawn {player.playerUsername} - player not dead");
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "RespawnError Error";
        CommandBody = $"Error Respawning Player";
    }
}

