using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class HelpCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public HelpCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return true;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Help";
        CommandBody = "Open Help Dialog";
         
        DialogueSegment[] dialog = 
        {
            new DialogueSegment
            {
                bodyText = "This is a fucking test",
                speakerText = "SAFETY COMPUTER",
                waitTime = 0f
            }
        } ;
        HUDManager.Instance.ReadDialogue(dialog);
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Help Error";
        CommandBody = $"Error setting Help";
    }
}

