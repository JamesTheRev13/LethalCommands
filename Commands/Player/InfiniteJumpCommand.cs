using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteJumpCommand : CommandBase
{
    
    public InfiniteJumpCommand()
    {
        CommandTitle = "Infinite Jump";
        ManualLogSource = Logger.CreateLogSource("InfiniteJumpCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteJump = !Plugin.Instance.infiniteJump;
        CommandBody = "Infinite Jump set to: " + Plugin.Instance.infiniteJump.ToString();
        ManualLogSource.LogInfo("Infinite Jump toggled to " + Plugin.Instance.infiniteJump.ToString());
        
    }
}

