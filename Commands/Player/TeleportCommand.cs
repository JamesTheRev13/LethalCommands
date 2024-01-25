using BepInEx.Logging;
using System;
using System.Linq;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class TeleportCommand : CommandBase
{
    
    public TeleportCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 2;
    }

    protected override void ExecuteCommand()
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;
        var matchedPlayer = 
            StartOfRound.Instance.allPlayerScripts
            .ToList()
            .Find(player => player.playerUsername.ToLower().Contains(parameters[1].ToLower())) ?? null;

        CommandTitle = $"Teleport {localPlayer.playerUsername}";
        switch (parameters[1].ToLower())
        {
            case "ship":

                break;

            default:
                if (matchedPlayer == null)
                {
                    CommandBody = $"Invalid Username: {parameters[1]}";
                    break;
                }    
                logger.LogInfo("Current player position: " + localPlayer.transform.position.ToString());
                logger.LogInfo($"Attempting to teleport {localPlayer.playerUsername} to {matchedPlayer.playerUsername} at position: {matchedPlayer.transform.position.ToString()}");
                localPlayer.TeleportPlayer(matchedPlayer.transform.position, false);
                logger.LogInfo($"Teleported {localPlayer.playerUsername}");
                break;
        }
        if (parameters[1].ToLower().Equals("ship"))
        {
            CommandBody = "Teleport " + localPlayer.playerUsername + " to Ship";
            logger.LogInfo("Current player position: " + localPlayer.transform.position.ToString());
            logger.LogInfo("Attempting to teleport " + localPlayer.playerUsername + " to position: " + StartOfRound.Instance.playerSpawnPositions[0].transform.position.ToString());
            localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
        }
        if (parameters[1].ToLower().Equals("inside"))
        {
            CommandBody = "Teleport " + localPlayer.playerUsername + " to Indoor Entrance";
            logger.LogInfo("Current player position: " + localPlayer.transform.position.ToString());
            logger.LogInfo("Attempting to teleport " + localPlayer.playerUsername + " to position: " + plugin.GetEntrance().ToString());

            localPlayer.TeleportPlayer(plugin.GetEntrance(), false);
        }
        if (parameters[1].ToLower().Equals("outside"))
        {
            CommandBody = "Teleport " + localPlayer.playerUsername + " to Outdoor Entrance";
            logger.LogInfo("Current player position: " + localPlayer.transform.position.ToString());
            logger.LogInfo("Attempting to teleport " + localPlayer.playerUsername + " to position: " + plugin.GetEntrance(true).ToString());

            localPlayer.TeleportPlayer(plugin.GetEntrance(true), false);
        }
        if (matchedPlayer != null)
        {
            
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Teleport Error";
        CommandBody = $"Error setting Teleport";
    }
}

