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
            plugin.movementSpeed = speed;
            CommandBody = "Movement Speed set to: " + plugin.movementSpeed.ToString();
        } else
        {
            // /speed - toggles speedHack
            plugin.speedHack = !plugin.speedHack;
            
            CommandBody = "SpeedHack set to: " + plugin.speedHack.ToString();
            logger.LogInfo("SpeedHack toggled to " + plugin.speedHack.ToString());
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "SpeedHack Error";
        CommandBody = $"Error setting SpeedHack";
    }
}

