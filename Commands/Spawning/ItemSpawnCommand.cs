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
        return parameters.Length == 2 || parameters.Length == 3;
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
            var spawnForAll = parameters[2]?.ToLower().Equals("all") ?? false;
            int count = 1;
            if (int.TryParse(parameters[2], out int count1)) {
                count = count1;
            } else if (int.TryParse(parameters[3], out int count2))
            {
                count = count2;
            }
            var matchedPlayer = StartOfRound.Instance.allPlayerScripts.ToList().First(player => 
            {
                bool matched = player.playerUsername.Contains(parameters[2] ?? null) && int.Parse(parameters[2] ?? null) == -1;
                return matched ? player : GameNetworkManager.Instance.localPlayerController;

            });
            var players = spawnForAll
                ? StartOfRound.Instance.allPlayerScripts.ToList()
                : new List<PlayerControllerB>() { matchedPlayer };

            players.ForEach(player =>
            {
                Vector3 spawnPosition = player.transform.position;
                if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                {
                    spawnPosition = player.spectatedPlayerScript.transform.position;
                }

                for (int i = 0; i < count; i++)
                {
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

