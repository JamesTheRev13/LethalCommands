using LethalCommands.Commands.Game;
using LethalCommands.Commands.Player;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LethalCommands.Commands;

public class CommandFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
            //"/respawn" => _serviceProvider.GetService<RespawnCommand>(),
            "/kill" => _serviceProvider.GetService<KillCommand>(),
            "/credits" => _serviceProvider.GetService<InfiniteCreditsCommand>(),
            "/deadline" => _serviceProvider.GetService<InfiniteDeadlineCommand>(),
            "/unlock" => _serviceProvider.GetService<UnlockAllDoorsCommand>(),
            //"/help" => _serviceProvider.GetService<HelpCommand>(),
            //"/terminal" => _serviceProvider.GetService<TerminalCommand>(),
            "/item" => _serviceProvider.GetService<ItemSpawnCommand>(),
            "/enemy" => _serviceProvider.GetService<EnemySpawnCommand>(),
            _ => throw new ArgumentException($"Invalid Command: {commandName}"),
        };
    }
}
