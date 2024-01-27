using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class NightVisionCommand : CommandBase
{
    public NightVisionCommand()
    {
        CommandTitle = "Night Vision";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        // TODO: Add support to customize night vision (color, range, intensity, etc..)
        Plugin.nightVision = !Plugin.nightVision;

        CommandBody = "Night Vision set to: " + Plugin.nightVision.ToString();
        ManualLogSource.LogInfo("Night Vision toggled to " + Plugin.nightVision.ToString());
    }
}

