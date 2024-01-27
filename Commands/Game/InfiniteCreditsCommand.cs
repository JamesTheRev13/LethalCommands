using BepInEx.Logging;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteCreditsCommand : CommandBase
{
    public InfiniteCreditsCommand()
    {
        CommandTitle = "Infinite Credits";
        ManualLogSource = Logger.CreateLogSource("InfiniteCreditsCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteCredits = !Plugin.Instance.infiniteCredits;
        CommandBody = "Infinite Credits set to: " + Plugin.Instance.infiniteCredits.ToString();

        ManualLogSource.LogInfo("Infinite Credits toggled to " + Plugin.Instance.infiniteCredits.ToString());
    }
}

