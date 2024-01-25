using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class UnlockAllDoorsCommand : CommandBase
{
    
    public UnlockAllDoorsCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {

    }

    protected override bool ValidateParameters()
    {
        // No parameters needed for this command
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Doors";
        CommandBody = "Unlocked All Doors";
        logger.LogInfo("Attempting to unlock all doors...");
        List<DoorLock> doorLocks = UnityEngine.Object.FindObjectsOfType<DoorLock>().ToList();

        foreach (DoorLock door in doorLocks)
        {
            plugin.logger.LogInfo("Found Door (" + door.GetInstanceID() + ") Locked? -> " + door.isLocked.ToString());
            if (door.isLocked)
            {
                door.UnlockDoorSyncWithServer();
                plugin.logger.LogInfo("Unlocked Door (" + door?.GetInstanceID() + ") -> " + door?.isLocked.ToString());
            }
        }
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Doors Error";
        CommandBody = "Error(s) Unlocking Doors";
    }
}

