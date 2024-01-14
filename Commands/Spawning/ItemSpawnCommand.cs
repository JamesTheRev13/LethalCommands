using BepInEx.Logging;
using System;
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
            Vector3 spawnPosition = GameNetworkManager.Instance.localPlayerController.transform.position;
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                spawnPosition = GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript.transform.position;
            }

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
                CommandBody = "Spawned " + count + " " + item.itemName + " at " + GameNetworkManager.Instance.localPlayerController.playerUsername;
            }
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

