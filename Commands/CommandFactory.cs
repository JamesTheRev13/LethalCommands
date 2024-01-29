using BepInEx.Logging;
using LethalCommands.Commands.Game;
using LethalCommands.Commands.Player;
using System;
using System.Collections.Generic;
using System.Linq;
// Command Pattern
namespace LethalCommands.Commands;

public class CommandFactory
{
    protected ManualLogSource logger;
    protected Plugin plugin;
    private Dictionary<string, Func<ICommand>> commandRegistry = new Dictionary<string, Func<ICommand>>();
    private Dictionary<string, ICommand> commandPool = new Dictionary<string, ICommand>();

    public CommandFactory(Plugin Instance)
    {
        plugin = Instance;
        logger = Instance.logger;
        // TODO: Add commands to a list and iterate through them to register them

        // TODO: Add command for unlimited carry weight
        // TODO: Add command to call lightnight strike...?
        // TODO: Add command for no fall damage
        // TODO: figure out how to enable/disble invites to lobby (join mid game or block invites/joins alltogether)
        // TODO: Add command to toggle fullbright
        // TODO: Add command to toggle fog
        // TODO: Add command to toggle ESP/walls...? (too lazy rn)
        // Player Commands
        RegisterCommand("/god", () => new GodModeCommand());
        RegisterCommand("/noclip", () => new NoClipCommand());
        RegisterCommand("/ammo", () => new InfiniteAmmoCommand());
        RegisterCommand("/vision", () => new NightVisionCommand());
        RegisterCommand("/sprint", () => new InfiniteSprintCommand());
        RegisterCommand("/speed", () => new SpeedHackCommand());
        RegisterCommand("/jumps", () => new InfiniteJumpCommand());
        RegisterCommand("/jump", () => new SuperJumpCommand());
        RegisterCommand("/teleport", () => new TeleportCommand());
        RegisterCommand("/battery", () => new InfiniteBatteryCommand());
        RegisterCommand("/respawn", () => new RespawnCommand());
        RegisterCommand("/kill", () => new KillCommand());

        // Game Commands
        RegisterCommand("/credits", () => new InfiniteCreditsCommand(   ));
        RegisterCommand("/deadline", () => new InfiniteDeadlineCommand());
        RegisterCommand("/unlock", () => new UnlockAllDoorsCommand());
        //RegisterCommand("/help", () => new HelpCommand());
        //RegisterCommand("/terminal", () => new TerminalCommand()); // not quite working as expected

        //Spawning Commands
        RegisterCommand("/item", () => new ItemSpawnCommand());
        RegisterCommand("/enemy", () => new EnemySpawnCommand());
    }
    public void RegisterCommand(string commandName, Func<ICommand> createCommand)
    {
        commandRegistry.Add(commandName, createCommand);
        logger.LogInfo($"Registered Command: {commandName}");
    }

    public ICommand CreateCommand(string inputCommand)
    {
        string command = inputCommand.Split(' ')[0];
        if (commandRegistry.TryGetValue(command, out var createCommand))
        {
            return createCommand();
        }
        return null;
    }

    public ICommand GetCommand(string inputCommand)
    {
        string commandText = inputCommand.Split(' ')[0].ToLower();
        if (!commandPool.ContainsKey(commandText))
        {
            // If the command does not exist in the pool, create it and add it to the pool
            ICommand command = CreateCommand(inputCommand);
            commandPool.Add(commandText, command);
            logger.LogInfo($"Added {commandText} to command pool.");
        }
        commandPool.ToList().ForEach(x => logger.LogInfo($"Command Pool: {x.Key}"));
        // Return the command from the pool
        return commandPool[commandText];
    }
}