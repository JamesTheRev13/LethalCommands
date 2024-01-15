using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteBatteryCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public InfiniteBatteryCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.infiniteBattery = !plugin.infiniteBattery;

        CommandTitle = "Infinite Battery";      
        CommandBody = "Infinite Battery set to: " + plugin.infiniteBattery.ToString();
        logger.LogInfo("Infinite Battery toggled to " + plugin.infiniteBattery.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Battery Error";
        CommandBody = $"Error setting Infinite Battery";
    }
}

