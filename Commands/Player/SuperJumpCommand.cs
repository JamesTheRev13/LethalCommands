using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SuperJumpCommand : CommandBase
{
    
    public SuperJumpCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Super Jump";

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
            logger.LogInfo("Super Jump toggled to " + Plugin.Instance.superJump.ToString());
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Super Jump Error";
        CommandBody = $"Error setting Super Jump";
    }
}

