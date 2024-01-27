﻿using BepInEx.Logging;
using System;

namespace LethalCommands.Commands;

public abstract class CommandBase : ICommand
{
    protected string[] parameters;

    protected ManualLogSource ManualLogSource;

    public string CommandTitle { get; protected set; } = string.Empty;
    public string CommandBody { get; protected set; } = string.Empty;
    public bool IsHostCommand { get; protected set; } = false;

    public CommandBase()
    {
        ManualLogSource = Plugin.Instance.logger;
    }

    public virtual void SetParameters(string inputCommand)
    {
        if (!ValidateParameters())
        {
            Plugin.Instance.logger.LogError("Invalid parameters for command: " + GetType().Name);
            throw new ArgumentException("Invalid parameters : " + GetCommand());
        }

        parameters = inputCommand.Split(' ');
        Plugin.Instance.logger.LogInfo("Parameters set for command: " + parameters);
    }
    
    public virtual string GetCommand()
    {
        return string.Join("", parameters);
    }

    protected abstract bool ValidateParameters();

    public void Execute()
    {
        Plugin.Instance.logger.LogInfo("Entered Execute() method");

        if (IsHostCommand && !GameNetworkManager.Instance.localPlayerController.IsHost)
        {
            throw new InvalidOperationException("You must be the host to run this command.");
        }
        
        Plugin.Instance.currentCommandIndex = -1;
        Plugin.Instance.commandHistory.Insert(0, GetCommand());
        Plugin.Instance.logger.LogInfo($"Added Command to Command History: {GetCommand()}");
        ExecuteCommand();
    }

    protected abstract void ExecuteCommand();
}

