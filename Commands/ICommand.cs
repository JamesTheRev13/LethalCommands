namespace LethalCommands.Commands;
// Command Pattern
public interface ICommand
{
    string CommandTitle { get; }
    string CommandBody { get; }

    void SetParameters(string inputCommand);
    void Execute();
}