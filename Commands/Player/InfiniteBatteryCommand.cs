using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteBatteryCommand : CommandBase
{
    public InfiniteBatteryCommand()
    {
        CommandTitle = "Infinite Battery";
        ManualLogSource = Logger.CreateLogSource("InfiniteBatteryCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteBattery = !Plugin.Instance.infiniteBattery;

        CommandBody = "Infinite Battery set to: " + Plugin.Instance.infiniteBattery.ToString();
        ManualLogSource.LogInfo("Infinite Battery toggled to " + Plugin.Instance.infiniteBattery.ToString());
        
    }
}

