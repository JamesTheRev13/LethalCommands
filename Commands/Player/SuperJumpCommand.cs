using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SuperJumpCommand : CommandBase
{
    public SuperJumpCommand()
    {
        CommandTitle = "Super Jump";
        ManualLogSource = Logger.CreateLogSource("SuperJumpCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {

        // /jump <force> - sets the jump force while superJump is enabled
        if (parameters.Length == 2)
        {
            var force = float.Parse(parameters[1]);
            Plugin.Instance.jumpForce = force;
            CommandBody = "Jump Force set to: " + Plugin.Instance.jumpForce.ToString();
        } else
        {
            // /jump - toggles superJump
            Plugin.Instance.superJump = !Plugin.Instance.superJump;
            
            CommandBody = "Super Jump set to: " + Plugin.Instance.superJump.ToString();
            ManualLogSource.LogInfo("Super Jump toggled to " + Plugin.Instance.superJump.ToString());
        }
    }
}

