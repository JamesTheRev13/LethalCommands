using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteAmmoCommand : CommandBase
{
    
    public InfiniteAmmoCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteAmmo = !Plugin. Instance.infiniteAmmo;

        CommandTitle = "Infinite Ammo";      
        CommandBody = "Infinite Ammo set to: " + Plugin.Instance.infiniteAmmo.ToString();
        logger.LogInfo("Infinite Ammo toggled to " + Plugin.Instance.infiniteAmmo.ToString());
        
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Infinite Ammo Error";
        CommandBody = $"Error setting Infinite Ammo";
    }
}

