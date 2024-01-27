using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteAmmoCommand : CommandBase
{
    public InfiniteAmmoCommand()
    {
        CommandTitle = "Infinite Ammo";
        ManualLogSource = Logger.CreateLogSource("InfiniteAmmoCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteAmmo = !Plugin. Instance.infiniteAmmo;

        CommandBody = "Infinite Ammo set to: " + Plugin.Instance.infiniteAmmo.ToString();
        ManualLogSource.LogInfo("Infinite Ammo toggled to " + Plugin.Instance.infiniteAmmo.ToString());
    }
}

