using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SuperJumpCommand : CommandBase
{
    // Constructor passing the logger to the base class
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
            plugin.jumpForce = force;
            CommandBody = "Jump Force set to: " + plugin.jumpForce.ToString();
        } else
        {
            // /jump - toggles superJump
            plugin.superJump = !plugin.superJump;
            
            CommandBody = "Super Jump set to: " + plugin.superJump.ToString();
            logger.LogInfo("Super Jump toggled to " + plugin.superJump.ToString());
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Super Jump Error";
        CommandBody = $"Error setting Super Jump";
    }
}

