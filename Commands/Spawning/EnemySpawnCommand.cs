using BepInEx.Logging;
using System;
using UnityEngine;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class EnemySpawnCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public EnemySpawnCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
        IsHostCommand = true;
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 2 || parameters.Length == 3;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Enemy Spawn";
        SpawnableEnemyWithRarity enemy = RoundManager.Instance.currentLevel.Enemies.Find(enemy => enemy.enemyType.enemyName.ToLower().Contains(parameters[1])); ;
        int enemyId = RoundManager.Instance.currentLevel.Enemies.IndexOf(enemy);

        Vector3 pos = GameNetworkManager.Instance.localPlayerController.transform.position;
        float yRot = GameNetworkManager.Instance.localPlayerController.transform.rotation.y;

        if (enemyId != -1)
        {
            int count = 1;

            if (parameters.Length == 3)
            {
                CommandBody = $"Spawned {count} {enemy.enemyType.enemyName}";
                try
                {
                    var countInput = int.Parse(parameters[2]);
                    if (countInput > 0)
                    {
                        count = countInput;
                    }
                }
                catch
                {
                    count = 0;
                    CommandBody = "Invalid Item Count";
                }
            }
            plugin.logger.LogInfo($"Attempting to spawn {count} {enemy.enemyType.enemyName} at player position: {pos}");
            for (int i = 0; i < count; i++)
            {
                RoundManager.Instance.SpawnEnemyOnServer(pos, yRot, enemyId);
                plugin.logger.LogInfo($"Attempted to spawn {enemy.enemyType.enemyName} #{i + 1} at player position: {pos}");
            }
        }
        else CommandBody = "Invalid Enemy: " + parameters[1];
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Enemy Spawn Error";
        CommandBody = $"Error Spawning Enemy";
    }
}

