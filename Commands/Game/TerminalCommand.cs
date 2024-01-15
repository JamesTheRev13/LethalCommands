using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class TerminalCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public TerminalCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Terminal";
        CommandBody = "Portable Terminal";
        logger.LogInfo("Accessing Terminal...");
        var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        terminal.BeginUsingTerminal();
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Terminal Error";
        CommandBody = "Error(s) Accessing Terminal";
    }
}

