using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteSprintCommand : CommandBase
{ 
    public InfiniteSprintCommand()
    {
        CommandTitle = "Infinite Sprint";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteSprint = !Plugin.infiniteSprint;
        CommandBody = "Infinite Sprint set to: " + Plugin.infiniteSprint.ToString();
        ManualLogSource.LogInfo("Infinite Sprint toggled to " + Plugin.infiniteSprint.ToString());
    }
}

