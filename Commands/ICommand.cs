namespace LethalCommands.Commands;
// Command Pattern
public interface ICommand
{
    string CommandTitle { get; }
    string CommandBody { get; }

    void SetParameters(string inputCommand);
    /// <summary>
    /// Gets the raw Command text with Parameters
    /// </summary>
    /// <returns>The full command string</returns>
    string GetCommand();

    void Execute();
}