using BepInEx.Logging;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteCreditsCommand : CommandBase
{
    public InfiniteCreditsCommand()
    {
        CommandTitle = "Infinite Credits";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteCredits = !Plugin.infiniteCredits;
        CommandBody = "Infinite Credits set to: " + Plugin.infiniteCredits.ToString();

        ManualLogSource.LogInfo("Infinite Credits toggled to " + Plugin.infiniteCredits.ToString());
    }
}

