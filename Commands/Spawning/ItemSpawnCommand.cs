using BepInEx.Logging;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class ItemSpawnCommand : CommandBase
{
    // Constructor passing the logger to the base class
    public ItemSpawnCommand(Plugin _plugin, ManualLogSource _logger) : base(_plugin, _logger)
    {
        IsHostCommand = true;
    }

    protected override bool ValidateParameters()
    {
        // /item shotgun
        // /item shotgun 3
        // /item shotgun all
        // /item shotgun BobSaget
        // /item shotgun all 2
        return parameters.Length >= 2 || parameters.Length <= 4;
    }

    protected override void ExecuteCommand()
    {
        CommandTitle = "Item Spawn";

        AllItemsList allItemsList = StartOfRound.Instance.allItemsList;
        allItemsList.itemsList
            .ForEach(item =>
            {
                plugin.logger.LogInfo("Item Name: " + item.itemName);
                plugin.logger.LogInfo("Item ID: " + item.itemId);
            }
        );

        Item item = allItemsList?.itemsList.Find(item => item.itemName.ToLower().Contains(parameters[1].Trim().ToLower())) ?? null;
        plugin.logger.LogInfo("Found Item: " + item?.itemName);

        if (item != null)
        {
            int count = 1;
            var spawnForAll = false;
            var matchedPlayer = GameNetworkManager.Instance.localPlayerController;

            if (parameters.Length > 2)
            {
                spawnForAll = parameters[2].ToLower().Equals("all");
                matchedPlayer = StartOfRound.Instance.allPlayerScripts.ToList().First(player =>
                {
                    bool matched = player.playerUsername.ToLower().Contains(parameters[2].ToLower()) && parameters[2].Length > 2;
                    return matched ? player : GameNetworkManager.Instance.localPlayerController;

                });
                if (int.TryParse(parameters[parameters.Length - 1], out int count1))
                {
                    count = count1;
                    plugin.logger.LogInfo("Parsed Count: " + count);
                }
            }
            plugin.logger.LogInfo("Spawn For All?: " + spawnForAll);
            plugin.logger.LogInfo("Matched Player?: " + matchedPlayer.playerUsername ?? "FALSE");

            var players = spawnForAll
                ? StartOfRound.Instance.allPlayerScripts.ToList().FindAll(player => !player.playerUsername.Contains("Player #")) // Filter out non-existent players
                : new List<PlayerControllerB>() { matchedPlayer };
            plugin.logger.LogInfo("New Player List Length: " + players.Count);
            players.ForEach(player =>
            {
                plugin.logger.LogInfo("Current Player iteration of Players: " + player.playerUsername);
                Vector3 spawnPosition = player.transform.position;
                if (player.isPlayerDead)
                {
                    spawnPosition = player.spectatedPlayerScript.transform.position;
                }

                for (int i = 0; i < count; i++)
                {
                    plugin.logger.LogInfo("Current Count iteration of Item Count: " + i);
                    GameObject itemObj = UnityEngine.Object.Instantiate(item.spawnPrefab, spawnPosition, Quaternion.identity);
                    itemObj.GetComponent<GrabbableObject>().fallTime = 0f;
                    int scrapValue = UnityEngine.Random.Range(60, 200);
                    itemObj.AddComponent<ScanNodeProperties>().scrapValue = scrapValue;
                    // setting a random scrap value for now, maybe make this configurable?
                    itemObj.GetComponent<GrabbableObject>().SetScrapValue(scrapValue);
                    if (parameters[1].ToLower().Contains("shotgun"))
                        itemObj.GetComponent<ShotgunItem>().shellsLoaded = plugin.infiniteAmmo ? 2147483647 : 2;
                    itemObj.GetComponent<NetworkObject>().Spawn();
                    plugin.logger.LogInfo($"Attempted to spawn {parameters[1]}!");
                    CommandBody = "Spawned " + count + " " + item.itemName + " at " + player.playerUsername;
                }
            });
        }
        else CommandBody = "Invalid Item: " + parameters[1];
    }

    protected override void HandleExecuteError(Exception ex)
    {
        base.HandleExecuteError(ex);
        CommandTitle = "Item Spawn Error";
        CommandBody = $"Error Spawning Item";
    }
}

