using BepInEx.Logging;
using LethalCommands.Commands.Game;
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

    public CommandFactory(Plugin Instance)
    {
        plugin = Instance;
        logger = Instance.logger;

        /* Register Commands During Init */

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
}