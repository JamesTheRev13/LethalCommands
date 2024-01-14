using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteJumpCommand : CommandBase
{
    // Constructor passing the logger to the base class
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
        plugin.infiniteJump = !plugin.infiniteJump;
        CommandTitle = "Infinite Jump";
        CommandBody = "Infinite Jump set to: " + plugin.infiniteJump.ToString();
        logger.LogInfo("Infinite Jump toggled to " + plugin.infiniteJump.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Jump Error";
        CommandBody = "Error toggling Infinite Jump";
    }
}

