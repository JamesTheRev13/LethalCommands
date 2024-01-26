using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteJumpCommand : CommandBase
{
    
    public InfiniteJumpCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteJump = !Plugin.Instance.infiniteJump;
        CommandTitle = "Infinite Jump";
        CommandBody = "Infinite Jump set to: " + Plugin.Instance.infiniteJump.ToString();
        logger.LogInfo("Infinite Jump toggled to " + Plugin.Instance.infiniteJump.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Jump Error";
        CommandBody = "Error toggling Infinite Jump";
    }
}

