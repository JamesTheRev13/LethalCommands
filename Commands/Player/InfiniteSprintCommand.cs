using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteSprintCommand : CommandBase
{ 
    public InfiniteSprintCommand()
    {
        CommandTitle = "Infinite Sprint";
        ManualLogSource = Logger.CreateLogSource("InfiniteSprintCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteSprint = !Plugin.Instance.infiniteSprint;
        CommandBody = "Infinite Sprint set to: " + Plugin.Instance.infiniteSprint.ToString();
        ManualLogSource.LogInfo("Infinite Sprint toggled to " + Plugin.Instance.infiniteSprint.ToString());
    }
}

