using BepInEx.Logging;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteDeadlineCommand : CommandBase
{
    public InfiniteDeadlineCommand()
    {
        IsHostCommand = true;
        CommandTitle = "Infinite Deadline";
        ManualLogSource = Logger.CreateLogSource("InfiniteDeadlineCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.Instance.infiniteDeadline = !Plugin.Instance.infiniteDeadline;
        CommandBody = "Infinite Deadline set to: " + Plugin.Instance.infiniteDeadline.ToString();

        ManualLogSource.LogInfo("Infinite Deadline toggled to " + Plugin.Instance.infiniteDeadline.ToString());
    }    
}

