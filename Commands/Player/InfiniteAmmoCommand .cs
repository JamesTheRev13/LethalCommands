using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteAmmoCommand : CommandBase
{
    public InfiniteAmmoCommand()
    {
        CommandTitle = "Infinite Ammo";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteAmmo = !Plugin.infiniteAmmo;

        CommandBody = "Infinite Ammo set to: " + Plugin.infiniteAmmo.ToString();
        ManualLogSource.LogInfo("Infinite Ammo toggled to " + Plugin.infiniteAmmo.ToString());
    }
}

