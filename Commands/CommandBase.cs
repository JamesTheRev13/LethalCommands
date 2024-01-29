using BepInEx.Logging;
using System;

namespace LethalCommands.Commands;

public abstract class CommandBase : ICommand
{
    protected string[] parameters;

    protected ManualLogSource ManualLogSource;
    protected Plugin Plugin;

    public string CommandTitle { get; protected set; } = string.Empty;
    public string CommandBody { get; protected set; } = string.Empty;
    public bool IsHostCommand { get; protected set; } = false;

    public CommandBase()
    {
        Plugin = Plugin.Instance;
        ManualLogSource = Plugin.logger;
    }

    public virtual void SetParameters(string inputCommand)
    {
        ManualLogSource.LogInfo("Entered SetParameters() method");
        parameters = inputCommand.Split(' ');
        ManualLogSource.LogInfo($"Valid Params? {ValidateParameters()}");
        if (!ValidateParameters())
        {
            ManualLogSource.LogError("Invalid parameters for command: " + GetType().Name);
            throw new ArgumentException("Invalid parameters : " + GetCommand());
        }

        ManualLogSource.LogInfo("Parameters set for command: " + GetCommand());
    }
    
    public virtual string GetCommand()
    {
        return string.Join("", parameters);
    }

    protected abstract bool ValidateParameters();

    public void Execute()
    {
        ManualLogSource.LogInfo("Entered Execute() method");

        if (IsHostCommand && !GameNetworkManager.Instance.localPlayerController.IsHost)
        {
            throw new InvalidOperationException("You must be the host to run this command.");
        }
        // TODO: remove command from history if it fails to execute
        // TODO: remove command from history if it already exists in history
        Plugin.currentCommandIndex = -1;
        Plugin.commandHistory.Insert(0, GetCommand());
        ManualLogSource.LogInfo($"Added Command to Command History: {GetCommand()}");
        ExecuteCommand();
    }

    protected abstract void ExecuteCommand();
}

