using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SuperJumpCommand : CommandBase
{
    public SuperJumpCommand()
    {
        CommandTitle = "Super Jump";
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
            Plugin.jumpForce = force;
            CommandBody = "Jump Force set to: " + Plugin.jumpForce.ToString();
        } else
        {
            // /jump - toggles superJump
            Plugin.superJump = !Plugin.superJump;
            
            CommandBody = "Super Jump set to: " + Plugin.superJump.ToString();
            ManualLogSource.LogInfo("Super Jump toggled to " + Plugin.superJump.ToString());
        }
    }
}

