using System;

namespace LethalCommands.Commands;
/// <summary>
/// The interface for all commands
/// </summary>
public interface ICommand
{
    /// <summary>
    /// The title of the command. Used in Command UI notification.
    /// </summary>
    string CommandTitle { get; }

    /// <summary>
    /// The body of the command. Used in Command UI notification.
    /// </summary>
    string CommandBody { get; }
    /// <summary>
    /// Sets the parameters for the command
    /// </summary>
    /// <param name="inputCommand">The raw command to extract parameters from. Ex: /item ammo all 3</param>
    void SetParameters(string inputCommand);

    /// <summary>
    /// Validates the parameters for the command
    /// </summary>
    /// <returns></returns>
    //bool ValidateParameters();

    /// <summary>
    /// Gets the raw Command text with Parameters
    /// </summary>
    /// <returns>The full command string</returns>
    string GetCommand();

    /// <summary>
    /// Executes the command
    /// </summary>
    void Execute();

    /// <summary>
    /// Handles unexpected command errors, and displays error to Command UI
    /// </summary>
    /// <param name="ex">The exception to be logged</param>
    //void HandleExecuteError(Exception ex);
}