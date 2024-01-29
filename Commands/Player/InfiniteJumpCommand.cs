using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteJumpCommand : CommandBase
{
    
    public InfiniteJumpCommand()
    {
        CommandTitle = "Infinite Jump";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteJump = !Plugin.infiniteJump;
        CommandBody = "Infinite Jump set to: " + Plugin.infiniteJump.ToString();
        ManualLogSource.LogInfo("Infinite Jump toggled to " + Plugin.infiniteJump.ToString());
        
    }
}

