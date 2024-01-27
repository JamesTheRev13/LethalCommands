using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class HelpCommand : CommandBase
{
    public HelpCommand()
    {
        CommandTitle = "Help";
        CommandBody = "Open Help Dialog";
        ManualLogSource = Logger.CreateLogSource("HelpCommand");
    }

    protected override bool ValidateParameters()
    {
        return true;
    }

    protected override void ExecuteCommand()
    {
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
}

