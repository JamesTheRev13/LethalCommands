using BepInEx.Logging;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class TerminalCommand : CommandBase
{
    public TerminalCommand()
    {
        CommandTitle = "Terminal";
        CommandBody = "Portable Terminal";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        ManualLogSource.LogInfo("Accessing Terminal...");
        var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        terminal.BeginUsingTerminal();
    }
}

