using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteCreditsCommand : CommandBase
{
    
    public InfiniteCreditsCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.infiniteCredits = !plugin.infiniteCredits;
        CommandTitle = "Infinite Credits";
        CommandBody = "Infinite Credits set to: " + plugin.infiniteCredits.ToString();
        logger.LogInfo("Infinite Credits toggled to " + plugin.infiniteCredits.ToString());

    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Credits Error";
        CommandBody = "Error toggling Infinite Credits";
    }
}

