using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SpeedHackCommand : CommandBase
{
    
    public SpeedHackCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "SpeedHack";

        // /speed <speed> - sets the movement speed while speedhack is enabled
        if (parameters.Length == 2)
        {
            var speed = float.Parse(parameters[1]);
            Plugin.Instance.movementSpeed = speed;
            CommandBody = "Movement Speed set to: " + Plugin.Instance.movementSpeed.ToString();
        } else
        {
            // /speed - toggles speedHack
            Plugin.Instance.speedHack = !Plugin.Instance.speedHack;
            
            CommandBody = "SpeedHack set to: " + Plugin.Instance.speedHack.ToString();
            logger.LogInfo("SpeedHack toggled to " + Plugin.Instance.speedHack.ToString());
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "SpeedHack Error";
        CommandBody = $"Error setting SpeedHack";
    }
}

