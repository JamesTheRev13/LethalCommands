using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class GodModeCommand : CommandBase
{
    //bool godMode;

    
    public GodModeCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
        //godMode = Instance.godMode;
    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        // /god would be 'param 1', anything afterwards is an illegal 'param2' etc...
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.godMode = !Plugin.Instance.godMode;
        CommandTitle = "God Mode";
        CommandBody = "God Mode set to: " + Plugin.Instance.godMode.ToString();
        logger.LogInfo("God mode toggled to " + Plugin.Instance.godMode.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "God Mode Error";
        CommandBody = "Error toggling God Mode";
    }
}

