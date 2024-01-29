using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteBatteryCommand : CommandBase
{
    public InfiniteBatteryCommand()
    {
        CommandTitle = "Infinite Battery";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteBattery = !Plugin.infiniteBattery;

        CommandBody = "Infinite Battery set to: " + Plugin.infiniteBattery.ToString();
        ManualLogSource.LogInfo("Infinite Battery toggled to " + Plugin.infiniteBattery.ToString());
        
    }
}

