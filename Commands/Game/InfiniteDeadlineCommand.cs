using BepInEx.Logging;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class InfiniteDeadlineCommand : CommandBase
{
    public InfiniteDeadlineCommand()
    {
        IsHostCommand = true;
        CommandTitle = "Infinite Deadline";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        Plugin.infiniteDeadline = !Plugin.infiniteDeadline;
        CommandBody = "Infinite Deadline set to: " + Plugin.infiniteDeadline.ToString();

        ManualLogSource.LogInfo("Infinite Deadline toggled to " + Plugin.infiniteDeadline.ToString());
    }    
}

