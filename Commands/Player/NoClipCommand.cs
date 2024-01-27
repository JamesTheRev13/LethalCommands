using BepInEx.Logging;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class NoClipCommand : CommandBase
{
    public NoClipCommand()
    {
        CommandTitle = "NoClip";
        ManualLogSource = Logger.CreateLogSource("NoClipCommand");
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length <= 2;
    }

    protected override void ExecuteCommand()
    {

        // /noclip <speed> - sets the speed of noclip
        if (parameters.Length == 2)
        {
            var speed = float.Parse(parameters[1]);
            Plugin.Instance.noclipSpeed = speed;
            CommandBody = "NoClip speed set to: " + Plugin.Instance.noclipSpeed.ToString();
        }
        else
        {
            // /noclip - toggles noclip
            Plugin.Instance.noclip = !Plugin.Instance.noclip;

            CommandBody = "NoClip set to: " + Plugin.Instance.noclip.ToString();
            ManualLogSource.LogInfo("NoClip toggled to " + Plugin.Instance.noclip.ToString());
        }
    }
}

