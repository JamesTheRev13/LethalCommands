using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class NightVisionCommand : CommandBase
{
    
    public NightVisionCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        // TODO: Add support to customize night vision (color, range, intensity, etc..)
        Plugin. Instance.nightVision = !Plugin.Instance.nightVision;

        CommandTitle = "Night Vision";
        CommandBody = "Night Vision set to: " + Plugin.Instance.nightVision.ToString();
        logger.LogInfo("Night Vision toggled to " + Plugin.Instance.nightVision.ToString());
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Night Vision Error";
        CommandBody = $"Error setting Night Vision";
    }
}

