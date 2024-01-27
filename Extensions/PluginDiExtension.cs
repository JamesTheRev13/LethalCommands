using LethalCommands.Commands.Game;
using LethalCommands.Commands.Player;
using Microsoft.Extensions.DependencyInjection;

namespace LethalCommands.Extensions
{
    public static class PluginDiExtension
    {
        public static IServiceCollection AddCommands(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<GodModeCommand>();
            serviceCollection.AddSingleton<InfiniteAmmoCommand>();
            serviceCollection.AddSingleton<InfiniteBatteryCommand>();
            serviceCollection.AddSingleton<InfiniteCreditsCommand>();
            serviceCollection.AddSingleton<InfiniteDeadlineCommand>();
            serviceCollection.AddSingleton<InfiniteJumpCommand>();
            serviceCollection.AddSingleton<InfiniteSprintCommand>();
            serviceCollection.AddSingleton<ItemSpawnCommand>();
            serviceCollection.AddSingleton<KillCommand>();
            serviceCollection.AddSingleton<NightVisionCommand>();
            serviceCollection.AddSingleton<NoClipCommand>();
            serviceCollection.AddSingleton<RespawnCommand>();
            serviceCollection.AddSingleton<SpeedHackCommand>();
            serviceCollection.AddSingleton<SuperJumpCommand>();
            serviceCollection.AddSingleton<TeleportCommand>();
            serviceCollection.AddSingleton<UnlockAllDoorsCommand>();
            serviceCollection.AddSingleton<EnemySpawnCommand>();
            serviceCollection.AddSingleton<HelpCommand>();
            serviceCollection.AddSingleton<TerminalCommand>();

            return serviceCollection;
        }
    }
}
