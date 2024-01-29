using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;

namespace LethalCommands.Commands.Game;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class UnlockAllDoorsCommand : CommandBase
{
    
    public UnlockAllDoorsCommand()
    {
        CommandTitle = "Doors";
        CommandBody = "Unlocked All Doors";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        ManualLogSource.LogInfo("Attempting to unlock all doors...");
        List<DoorLock> doorLocks = UnityEngine.Object.FindObjectsOfType<DoorLock>().ToList();

        foreach (DoorLock door in doorLocks)
        {
            ManualLogSource.LogInfo("Found Door (" + door.GetInstanceID() + ") Locked? -> " + door.isLocked.ToString());
            if (door.isLocked)
            {
                door.UnlockDoorSyncWithServer();
                ManualLogSource.LogInfo("Unlocked Door (" + door?.GetInstanceID() + ") -> " + door?.isLocked.ToString());
            }
        }
    }
}

