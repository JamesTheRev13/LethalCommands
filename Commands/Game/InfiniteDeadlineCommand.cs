using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteDeadlineCommand : CommandBase
{
    
    public InfiniteDeadlineCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
        IsHostCommand = true;
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.infiniteDeadline = !plugin.infiniteDeadline;
        CommandTitle = "Infinite Deadline";
        CommandBody = "Infinite Deadline set to: " + plugin.infiniteDeadline.ToString();
        logger.LogInfo("Infinite Deadline toggled to " + plugin.infiniteDeadline.ToString());

    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Deadline Error";
        CommandBody = "Error toggling Infinite Deadline";
    }
}

