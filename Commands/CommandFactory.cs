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
    public CommandFactory(Plugin plugin, ManualLogSource logger)
    {
        this.plugin = plugin;
        this.logger = logger;

        /* Register Commands During Init */

        // Player Commands
        RegisterCommand("/god", () => new GodModeCommand(plugin, logger));
        RegisterCommand("/noclip", () => new NoClipCommand(plugin, logger));
        RegisterCommand("/ammo", () => new InfiniteAmmoCommand(plugin, logger));
        RegisterCommand("/vision", () => new NightVisionCommand(plugin, logger));
        RegisterCommand("/sprint", () => new InfiniteSprintCommand(plugin, logger));
        RegisterCommand("/speed", () => new SpeedHackCommand(plugin, logger));
        RegisterCommand("/jumps", () => new InfiniteJumpCommand(plugin, logger));
        RegisterCommand("/jump", () => new SuperJumpCommand(plugin, logger));
        RegisterCommand("/teleport", () => new TeleportCommand(plugin, logger));
        RegisterCommand("/battery", () => new InfiniteBatteryCommand(plugin, logger));
        RegisterCommand("/respawn", () => new RespawnCommand(plugin, logger));
        RegisterCommand("/kill", () => new KillCommand(plugin, logger));

        // Game Commands
        RegisterCommand("/credits", () => new InfiniteCreditsCommand(plugin, logger));
        RegisterCommand("/deadline", () => new InfiniteDeadlineCommand(plugin, logger));
        RegisterCommand("/unlock", () => new UnlockAllDoorsCommand(plugin, logger));
        //RegisterCommand("/help", () => new HelpCommand(plugin, logger));
        //RegisterCommand("/terminal", () => new TerminalCommand(plugin, logger)); // not quite working as expected

        //Spawning Commands
        RegisterCommand("/item", () => new ItemSpawnCommand(plugin, logger));
        RegisterCommand("/enemy", () => new EnemySpawnCommand(plugin, logger));
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
        logger.LogError("Unknown command: " + inputCommand);
        HUDManager.Instance.DisplayTip("Error", "Unknown Command");
        HUDManager.Instance.chatTextField.text = "";
        return null;
    }
}
