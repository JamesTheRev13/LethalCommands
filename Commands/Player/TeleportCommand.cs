using BepInEx.Logging;
using System;
using System.Linq;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class TeleportCommand : CommandBase
{
    public TeleportCommand()
    {
        CommandTitle = $"Teleport {GameNetworkManager.Instance.localPlayerController.playerUsername}";
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

        switch (parameters[1].ToLower())
        {
            case "ship":
                CommandBody = $"Teleport {localPlayer.playerUsername} to Ship";
                ManualLogSource.LogInfo($"Current player position: {localPlayer.transform.position.ToString()}");
                ManualLogSource.LogInfo($"Attempting to teleport {localPlayer.playerUsername} to position: {StartOfRound.Instance.playerSpawnPositions[0].transform.position}");
                localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
                return;
            case "inside":
                CommandBody = $"Teleport {localPlayer.playerUsername} to Indoor Entrance";
                ManualLogSource.LogInfo($"Current player position: {localPlayer.transform.position.ToString()}");
                ManualLogSource.LogInfo($"Attempting to teleport {localPlayer.playerUsername} to position: {Plugin.GetEntrance()}");

                localPlayer.TeleportPlayer(Plugin.GetEntrance(), false);
                return;
            case "outside":
                CommandBody = $"Teleport {localPlayer.playerUsername} to Outdoor Entrance";
                ManualLogSource.LogInfo($"Current player position: {localPlayer.transform.position.ToString()}");
                ManualLogSource.LogInfo($"Attempting to teleport {localPlayer.playerUsername} to position: {Plugin.GetEntrance(true)}");

                localPlayer.TeleportPlayer(Plugin.GetEntrance(true), false);
                return;
            default:
                if (matchedPlayer != null)
                {
                    ManualLogSource.LogInfo($"Current player position: {localPlayer.transform.position}");
                    ManualLogSource.LogInfo($"Attempting to teleport {localPlayer.playerUsername} to {matchedPlayer.playerUsername} at position: {matchedPlayer.transform.position}");
                    CommandBody = $"Teleport {localPlayer.playerUsername} to {matchedPlayer.playerUsername}";
                    localPlayer.TeleportPlayer(matchedPlayer.transform.position, false);
                    return;
                }
                throw new ArgumentException($"Invalid Teleport Command: {GetCommand()}");
        }
    }
}

