using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteSprintCommand : CommandBase
{
    
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
        Plugin.Instance.infiniteSprint = !Plugin.Instance.infiniteSprint;
        CommandTitle = "Infinite Sprint";
        CommandBody = "Infinite Sprint set to: " + Plugin.Instance.infiniteSprint.ToString();
        logger.LogInfo("Infinite Sprint toggled to " + Plugin.Instance.infiniteSprint.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Sprint Error";
        CommandBody = "Error toggling Infinite Sprint";
    }
}

