using BepInEx.Logging;
using LethalCommands.Commands.Player;
using System;
using System.Collections.Generic;
// Command Pattern
namespace LethalCommands.Commands;

public class CommandFactory
{
    protected ManualLogSource logger;
    protected Plugin plugin;
    private Dictionary<string, Func<ICommand>> commandRegistry = new Dictionary<string, Func<ICommand>>();

    public CommandFactory(Plugin plugin, ManualLogSource logger)
    {
        this.plugin = plugin;
        this.logger = logger;

        // Register Commands During Init
        RegisterCommand("/god", () => new GodModeCommand(plugin, logger));
        RegisterCommand("/noclip", () => new NoClipCommand(plugin, logger));

    }
    public void RegisterCommand(string commandName, Func<ICommand> createCommand)
    {
        commandRegistry.Add(commandName, createCommand);
    }

    public ICommand CreateCommand(string inputCommand)
    {
        foreach (var command in commandRegistry)
        {
            // This might be dangerous to 'loosely' link commands... keep an eye on this
            if (inputCommand.Contains(command.Key))
            {
                return command.Value();
            }
        }
        logger.LogError("Unknown command: " + inputCommand);
        return null;
    }
}
