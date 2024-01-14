using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteAmmoCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public InfiniteAmmoCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        plugin.infiniteAmmo = !plugin.infiniteAmmo;

        CommandTitle = "Infinite Ammo";      
        CommandBody = "Infinite Ammo set to: " + plugin.infiniteAmmo.ToString();
        logger.LogInfo("Infinite Ammo toggled to " + plugin.infiniteAmmo.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Ammo Error";
        CommandBody = $"Error setting Infinite Ammo";
    }
}

