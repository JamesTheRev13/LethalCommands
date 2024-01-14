using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class GodModeCommand : CommandBase
{
    //bool godMode;

    // Constructor passing the logger to the base class
    public GodModeCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
        //godMode = plugin.godMode;
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        // /god would be 'param 1', anything afterwards is an illegal 'param2' etc...
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.godMode = !plugin.godMode;
        CommandTitle = "God Mode";
        CommandBody = "God Mode set to: " + plugin.godMode.ToString();
        logger.LogInfo("God mode toggled to " + plugin.godMode.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "God Mode Error";
        CommandBody = "Error toggling God Mode";
    }
}

