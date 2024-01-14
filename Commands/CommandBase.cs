using BepInEx.Logging;
using System;
// Command Pattern
namespace LethalCommands.Commands;

public abstract class CommandBase : ICommand
{
    protected string[] parameters;
    protected ManualLogSource logger;
    protected Plugin plugin;

    public string CommandTitle { get; protected set; }
    public string CommandBody { get; protected set; }

    public CommandBase(Plugin plugin, ManualLogSource logger)
    {
        this.logger = logger;
        this.plugin = plugin;
    }
    public virtual void SetParameters(string inputCommand)
    {
        try
        {
            parameters = inputCommand.Split(' ');

            if (!ValidateParameters())
            {
                HandleInvalidParameters();
            }
        }
        catch (Exception ex)
        {
            HandleUnexpectedError(ex);
        }
    }

    protected abstract bool ValidateParameters();

    protected virtual void HandleInvalidParameters()
    {
        logger.LogError("Invalid parameters for command: " + GetType().Name);
    }

    protected virtual void HandleUnexpectedError(Exception ex)
    {
        logger.LogError("Unexpected error in command: " + GetType().Name + "\n" + ex.Message);
    }

    public void Execute()
    {
        try
        {
            // Call the specific logic in the derived class
            ExecuteCommand();
            DisplayCommandLog();
        }
        catch (Exception ex)
        {
            HandleExecuteError(ex);
        }
    }

    protected abstract void ExecuteCommand();

    protected virtual void DisplayCommandLog()
    {
        HUDManager.Instance.DisplayTip(CommandTitle, CommandBody);
    }

    protected virtual void HandleExecuteError(Exception ex)
    {
        logger.LogError("Unexpected error in command execution: " + GetType().Name + "\n" + ex.Message);
    }
}

