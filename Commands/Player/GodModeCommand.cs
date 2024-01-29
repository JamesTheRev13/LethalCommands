using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class GodModeCommand : CommandBase
{
    public GodModeCommand()
    {
        CommandTitle = "God Mode";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.godMode = !Plugin.godMode;

        CommandBody = $"God Mode set to: {Plugin.godMode}";
        ManualLogSource.LogInfo($"God mode toggled to {Plugin.godMode}");
    }
}

