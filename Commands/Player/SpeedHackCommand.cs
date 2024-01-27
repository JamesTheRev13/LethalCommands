using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class SpeedHackCommand : CommandBase
{
    
    public SpeedHackCommand()
    {
        CommandTitle = "SpeedHack";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {
        // /speed <speed> - sets the movement speed while speedhack is enabled
        if (parameters.Length == 2)
        {
            var speed = float.Parse(parameters[1]);
            Plugin.movementSpeed = speed;
            CommandBody = "Movement Speed set to: " + Plugin.movementSpeed.ToString();
        } else
        {
            // /speed - toggles speedHack
            Plugin.speedHack = !Plugin.speedHack;
            
            CommandBody = "SpeedHack set to: " + Plugin.speedHack.ToString();
            ManualLogSource.LogInfo("SpeedHack toggled to " + Plugin.speedHack.ToString());
        }
    }
}

