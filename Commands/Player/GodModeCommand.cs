using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class GodModeCommand : CommandBase
{
    public GodModeCommand()
    {
        CommandTitle = "God Mode";
        ManualLogSource = Plugin.Instance.logger;
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.godMode = !Plugin.Instance.godMode;

        CommandBody = $"God Mode set to: {Plugin.Instance.godMode}";
        ManualLogSource.LogInfo($"God mode toggled to {Plugin.Instance.godMode}");
    }
}

