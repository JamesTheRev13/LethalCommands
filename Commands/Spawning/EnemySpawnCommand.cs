using BepInEx.Logging;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class EnemySpawnCommand : CommandBase
{
    
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
        var insideEnemies = RoundManager.Instance.currentLevel.Enemies.ToList();
        var outsideEnemies = RoundManager.Instance.currentLevel.OutsideEnemies.ToList();
        var enemies = insideEnemies.Concat(outsideEnemies).ToList();

        insideEnemies.ForEach(e => logger.LogInfo($"SpawnableEnemy - Inside: {e.enemyType.enemyName} -> ID: {insideEnemies.IndexOf(e)}"));
        outsideEnemies.ForEach(e => logger.LogInfo($"SpawnableEnemy - Outside: {e.enemyType.enemyName} -> ID: {outsideEnemies.IndexOf(e)}"));

        SpawnableEnemyWithRarity insideEnemy = insideEnemies.Find(enemy => enemy.enemyType.enemyName.ToLower().Contains(parameters[1])) ?? null;
        SpawnableEnemyWithRarity outsideEnemy = outsideEnemies.Find(enemy => enemy.enemyType.enemyName.ToLower().Contains(parameters[1])) ?? null;
        bool isInside = insideEnemy != null;
        int enemyId = isInside ? insideEnemies.IndexOf(insideEnemy) : outsideEnemies.IndexOf(outsideEnemy);
        logger.LogInfo($"Spawning SpawnableEnemy: {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName )}");

        Vector3 pos = GameNetworkManager.Instance.localPlayerController.transform.position;
        float yRot = GameNetworkManager.Instance.localPlayerController.transform.rotation.y;

        if (enemyId != -1)
        {
            int count = 1;

            if (parameters.Length == 3)
            {
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
            Plugin.Instance.logger.LogInfo($"Attempting to spawn {count} {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName)} at player position: {pos}");
            for (int i = 0; i < count; i++)
            {
                // Outside enemies spawn differently for some reason
                if (!isInside)
                {
                    GameObject obj = UnityEngine.Object.Instantiate(outsideEnemies[enemyId].enemyType.enemyPrefab, pos, Quaternion.Euler(Vector3.zero));
                    obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
                } else RoundManager.Instance.SpawnEnemyOnServer(pos, yRot, enemyId);

                Plugin. Instance.logger.LogInfo($"Attempted to spawn {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName)} #{i + 1} at player position: {pos}");
            }
            CommandBody = $"Spawned {count} {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName)}";
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

