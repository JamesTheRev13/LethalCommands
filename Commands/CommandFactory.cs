using BepInEx.Logging;
using LethalCommands.Commands.Game;
using LethalCommands.Commands.Player;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
// Command Pattern
namespace LethalCommands.Commands;

public class CommandFactory
{
    private readonly Plugin _plugin;
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Func<ICommand>> _commandRegistry = new Dictionary<string, Func<ICommand>>();

    public CommandFactory(Plugin plugin, IServiceProvider serviceProvider)
    {
        _plugin = plugin;
        _serviceProvider = serviceProvider;

        /* Register Commands During Init */

        // Player Commands
        //RegisterCommand("/god", () => new GodModeCommand(plugin));
        //RegisterCommand("/noclip", () => new NoClipCommand(plugin));
        //RegisterCommand("/ammo", () => new InfiniteAmmoCommand(plugin));
        //RegisterCommand("/vision", () => new NightVisionCommand(plugin));
        //RegisterCommand("/sprint", () => new InfiniteSprintCommand(plugin));
        //RegisterCommand("/speed", () => new SpeedHackCommand(plugin));
        //RegisterCommand("/jumps", () => new InfiniteJumpCommand(plugin));
        //RegisterCommand("/jump", () => new SuperJumpCommand(plugin));
        //RegisterCommand("/teleport", () => new TeleportCommand(plugin));
        //RegisterCommand("/battery", () => new InfiniteBatteryCommand(plugin));
        //RegisterCommand("/respawn", () => new RespawnCommand(plugin));
        //RegisterCommand("/kill", () => new KillCommand(plugin));

        //// Game Commands
        //RegisterCommand("/credits", () => new InfiniteCreditsCommand(plugin));
        //RegisterCommand("/deadline", () => new InfiniteDeadlineCommand(plugin));
        //RegisterCommand("/unlock", () => new UnlockAllDoorsCommand(plugin));
        ////RegisterCommand("/help", () => new HelpCommand(plugin));
        ////RegisterCommand("/terminal", () => new TerminalCommand(plugin)); // not quite working as expected

        ////Spawning Commands
        //RegisterCommand("/item", () => new ItemSpawnCommand(plugin));
        //RegisterCommand("/enemy", () => new EnemySpawnCommand(plugin));
    }
    //public void RegisterCommand(string commandName, Func<ICommand> createCommand)
    //{
    //    commandRegistry.Add(commandName, createCommand);
    //    plugin.logger.LogInfo($"Registered Command: {commandName}");
    //}

    //public ICommand CreateCommand(string inputCommand)
    //{
    //    string command = inputCommand.Split(' ')[0];
    //    if (commandRegistry.TryGetValue(command, out var createCommand))
    //    {
    //        return createCommand();
    //    }
    //    plugin.logger.LogError("Unknown command: " + inputCommand);
    //    HUDManager.Instance.DisplayTip("Error", "Unknown Command");
    //    HUDManager.Instance.chatTextField.text = "";
    //    return null;
    //}

    public ICommand GetCommand(string commandName)
    {
        string command = commandName.Split(' ')[0];

        return command switch
        {
            "/god" => _serviceProvider.GetService<GodModeCommand>(),
            "/noclip" => _serviceProvider.GetService<NoClipCommand>(),
            "/ammo" => _serviceProvider.GetService<InfiniteAmmoCommand>(),
            "/vision" => _serviceProvider.GetService<NightVisionCommand>(),
            "/sprint" => _serviceProvider.GetService<InfiniteSprintCommand>(),
            "/speed" => _serviceProvider.GetService<SpeedHackCommand>(),
            "/jumps" => _serviceProvider.GetService<InfiniteJumpCommand>(),
            "/jump" => _serviceProvider.GetService<SuperJumpCommand>(),
            "/teleport" => _serviceProvider.GetService<TeleportCommand>(),
            "/battery" => _serviceProvider.GetService<InfiniteBatteryCommand>(),
            "/respawn" => _serviceProvider.GetService<RespawnCommand>(),
            "/kill" => _serviceProvider.GetService<KillCommand>(),
            "/credits" => _serviceProvider.GetService<InfiniteCreditsCommand>(),
            "/deadline" => _serviceProvider.GetService<InfiniteDeadlineCommand>(),
            "/unlock" => _serviceProvider.GetService<UnlockAllDoorsCommand>(),
            "/help" => _serviceProvider.GetService<HelpCommand>(),
            "/terminal" => _serviceProvider.GetService<TerminalCommand>(),
            "/item" => _serviceProvider.GetService<ItemSpawnCommand>(),
            "/enemy" => _serviceProvider.GetService<EnemySpawnCommand>(),
            _ => throw new ArgumentException($"Invalid Command: {commandName}"),
        };
    }
}
