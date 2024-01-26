﻿using BepInEx.Logging;
using System;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class NoClipCommand : CommandBase
{
    
    public NoClipCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "NoClip";

        // /noclip <speed> - sets the speed of noclip
        if (parameters.Length == 2)
        {
            var speed = float.Parse(parameters[1]);
            Plugin.Instance.noclipSpeed = speed;
            CommandBody = "NoClip speed set to: " + Plugin.Instance.noclipSpeed.ToString();
        } else
        {
            // /noclip - toggles noclip
            Plugin.Instance.noclip = !Plugin.Instance.noclip;
            
            CommandBody = "NoClip set to: " + Plugin.Instance.noclip.ToString();
            logger.LogInfo("NoClip toggled to " + Plugin.Instance.noclip.ToString());
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "NoClip Error";
        CommandBody = $"Error setting NoClip";
    }
}

