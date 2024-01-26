using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCommands.Commands;
using LethalCommands.Patches;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/* PLUGIN BY BOB SAGET -  INSPIRED BY GAMEMASTER, DANCETOOLS, and NON-LETHAL-COMPANY - VERY EARLY WORK IN PROGRESS */
namespace LethalCommands;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public ManualLogSource logger;
    public static Plugin Instance;
    // Command Pattern - https://refactoring.guru/design-patterns/command
    public CommandFactory commandFactory;

    #region Command Fields
    public List<string> commandHistory = new();
    public int currentCommandIndex = -1;
    public bool noclip = false;
    public bool godMode = false;
    //public bool demiGod = false;
    //public bool invisibility = false;
    public bool infiniteAmmo = false;
    public bool nightVision = false;
    public bool speedHack = false;
    public bool infiniteSprint = false;
    public bool infiniteJump = false;
    public bool infiniteCredits = false;
    public bool infiniteDeadline = false;
    public bool infiniteBattery = false;
    public bool superJump = false;
    public float jumpForce = (float)13.0;
    public float movementSpeed = (float)4.6;
    public float noclipSpeed = 10.0f;
    public float nightVisionIntensity = 1000f;
    public float nightVisionRange = 10000f;
    public Color nightVisionColor = Color.green;
    #endregion
    private void Awake()
    {
        logger = Logger;
        Instance = this;
        commandFactory = new CommandFactory(Instance, logger);

        logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        // There has to be a better way to do this lol
        Harmony.CreateAndPatchAll(typeof(Plugin));
        Harmony.CreateAndPatchAll(typeof(PlayerControllerBPatches));
        Harmony.CreateAndPatchAll(typeof(QuickMenuManagerPatches));
        Harmony.CreateAndPatchAll(typeof(GrabbableObjectPatches));
        Harmony.CreateAndPatchAll(typeof(ShotgunItemPatches));
        Harmony.CreateAndPatchAll(typeof(TerminalPatches));
        Harmony.CreateAndPatchAll(typeof(TimeOfDayPatches));
        Harmony.CreateAndPatchAll(typeof(RoundManagerPatches));
        Harmony.CreateAndPatchAll(typeof(HUDManagerPatches));

        logger.LogInfo($"{PluginInfo.PLUGIN_GUID} patched!");
    }

    public Vector3 GetEntrance(bool getOutsideEntrance = false)
    {
        EntranceTeleport[] array = FindObjectsOfType<EntranceTeleport>(includeInactive: false);
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].entranceId != 0)
            {
                continue;
            }

            if (!getOutsideEntrance)
            {
                if (!array[i].isEntranceToBuilding)
                {
                    return array[i].entrancePoint.position;

                }
            }
            else if (array[i].isEntranceToBuilding)
            {
                return array[i].entrancePoint.position;
            }
        }

        Debug.LogError("Main entrance position could not be found. Returning origin.");
        return Vector3.zero;
    }
    
    public void Debug_SpawnItem(int itemToSpawnId, Transform[] debugEnemySpawnPositions)
    {
        if (NetworkManager.Singleton.IsConnectedClient && NetworkManager.Singleton.IsServer)
        {
            GameObject obj = Instantiate(StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].spawnPrefab, GameNetworkManager.Instance.localPlayerController.transform.position, Quaternion.identity, StartOfRound.Instance.propsContainer);
            obj.GetComponent<GrabbableObject>().fallTime = 0f;
            obj.GetComponent<NetworkObject>().Spawn();
            logger.LogInfo($"[DEBUG MENU] Spawned {StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].itemName} at {GameNetworkManager.Instance.localPlayerController.transform.position}");
        }
    }

    public void Debug_SpawnEnemy(int enemyTypeId, SelectableLevel testAllEnemiesLevel, int enemyToSpawnId, int numberEnemyToSpawn, Transform[] debugEnemySpawnPositions)
    {
        if (!NetworkManager.Singleton.IsConnectedClient || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        EnemyType enemyType = null;
        Vector3 spawnPosition = Vector3.zero;
        switch (enemyTypeId)
        {
            case 0:
                enemyType = testAllEnemiesLevel.Enemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.insideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.insideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
            case 1:
                enemyType = testAllEnemiesLevel.OutsideEnemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
            case 2:
                enemyType = testAllEnemiesLevel.DaytimeEnemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
        }

        if (!(enemyType == null))
        {
            for (int i = 0; i < numberEnemyToSpawn && i <= 50; i++)
            {
                RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, 0f, -1, enemyType);
                logger.LogInfo($"[DEBUG MENU] Spawned {enemyType.enemyName} at {spawnPosition}");
            }
        }
    }
}