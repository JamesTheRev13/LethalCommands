using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class NightVisionCommand : CommandBase
{
    public NightVisionCommand()
    {
        CommandTitle = "Night Vision";
        ManualLogSource = Logger.CreateLogSource("NightVisionCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        // TODO: Add support to customize night vision (color, range, intensity, etc..)
        Plugin. Instance.nightVision = !Plugin.Instance.nightVision;

        CommandBody = "Night Vision set to: " + Plugin.Instance.nightVision.ToString();
        ManualLogSource.LogInfo("Night Vision toggled to " + Plugin.Instance.nightVision.ToString());
    }
}

