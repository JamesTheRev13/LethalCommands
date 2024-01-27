using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class KillCommand : CommandBase
{
    public KillCommand()
    {
        CommandTitle = "Kill";
        ManualLogSource = Logger.CreateLogSource("KillCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        var player = GameNetworkManager.Instance.localPlayerController;
        CommandBody = $"Killed {player.playerUsername}.";
        player.KillPlayer(player.transform.position, true);
        ManualLogSource.LogInfo($"Killed {player.playerUsername}");
    }
}

