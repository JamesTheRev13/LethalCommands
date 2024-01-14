using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteSprintCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public InfiniteSprintCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.infiniteSprint = !plugin.infiniteSprint;
        CommandTitle = "Infinite Sprint";
        CommandBody = "Infinite Sprint set to: " + plugin.infiniteSprint.ToString();
        logger.LogInfo("Infinite Sprint toggled to " + plugin.infiniteSprint.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Sprint Error";
        CommandBody = "Error toggling Infinite Sprint";
    }
}

