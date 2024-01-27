using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class EnemySpawnCommand : CommandBase
{
    
    public EnemySpawnCommand()
    {
        IsHostCommand = true;
        CommandTitle = "Enemy Spawn";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 2 || parameters.Length == 3;
    }

    protected override void ExecuteCommand()
    {
        var insideEnemies = RoundManager.Instance.currentLevel.Enemies.ToList();
        var outsideEnemies = RoundManager.Instance.currentLevel.OutsideEnemies.ToList();
        var enemies = insideEnemies.Concat(outsideEnemies).ToList();

        insideEnemies.ForEach(e => ManualLogSource.LogInfo($"SpawnableEnemy - Inside: {e.enemyType.enemyName} -> ID: {insideEnemies.IndexOf(e)}"));
        outsideEnemies.ForEach(e => ManualLogSource.LogInfo($"SpawnableEnemy - Outside: {e.enemyType.enemyName} -> ID: {outsideEnemies.IndexOf(e)}"));

        SpawnableEnemyWithRarity insideEnemy = insideEnemies.Find(enemy => enemy.enemyType.enemyName.ToLower().Contains(parameters[1])) ?? null;
        SpawnableEnemyWithRarity outsideEnemy = outsideEnemies.Find(enemy => enemy.enemyType.enemyName.ToLower().Contains(parameters[1])) ?? null;
        bool isInside = insideEnemy != null;
        int enemyId = isInside ? insideEnemies.IndexOf(insideEnemy) : outsideEnemies.IndexOf(outsideEnemy);
        ManualLogSource.LogInfo($"Spawning SpawnableEnemy: {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName )}");

        Vector3 pos = GameNetworkManager.Instance.localPlayerController.transform.position;
        float yRot = GameNetworkManager.Instance.localPlayerController.transform.rotation.y;

        if (enemyId == -1)
            throw new ArgumentException($"Invalid Enemy: {parameters[1]}");
        
        int count = 1;

        if (parameters.Length == 3)
        {
            var countInput = int.Parse(parameters[2]);
            if (countInput < 1)
            {
                throw new ArgumentException($"Invalid Count: {parameters[2]}");
            }
            count = countInput;
        }
        ManualLogSource.LogInfo($"Attempting to spawn {count} {(isInside ? insideEnemy.enemyType.enemyName : outsideEnemy.enemyType.enemyName)} at player position: {pos}");
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
}

