
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(QuickMenuManager))]
public class QuickMenuManagerPatches
{
    [HarmonyPatch(nameof(QuickMenuManager.Debug_SpawnItem))]
    [HarmonyPrefix]
    static bool DebugSpawnItemOverride(ref int ___itemToSpawnId, ref Transform[] ___debugEnemySpawnPositions)
    {
        Plugin.Instance.Debug_SpawnItem(___itemToSpawnId, ___debugEnemySpawnPositions);

        return false;
    }

    [HarmonyPatch(nameof(QuickMenuManager.Start))]
    [HarmonyPrefix]
    static void EnableDebugMenu()
    {
        var menuManager = Object.FindObjectOfType<QuickMenuManager>();
        menuManager.Debug_SetEnemyDropdownOptions();
        menuManager.Debug_SetAllItemsDropdownOptions();
    }

    [HarmonyPatch(nameof(QuickMenuManager.Debug_ToggleTestRoom))]
    [HarmonyPrefix]
    static bool DebugTestRoomOverride()
    {
        StartOfRound.Instance.Debug_EnableTestRoomServerRpc(StartOfRound.Instance.testRoom == null);
        Plugin.Instance.logger.LogInfo($"[DEBUG MENU] Test Room: {StartOfRound.Instance.testRoom == null}");

        return false;
    }

    [HarmonyPatch(nameof(QuickMenuManager.Debug_ToggleAllowDeath))]
    [HarmonyPrefix]
    static bool DebugAllowDeathOverride()
    {
        StartOfRound.Instance.Debug_ToggleAllowDeathServerRpc();
        Plugin.Instance.logger.LogInfo($"[DEBUG MENU] God Mode: {StartOfRound.Instance.allowLocalPlayerDeath}");
        return false;
    }

    [HarmonyPatch(nameof(QuickMenuManager.Debug_SpawnEnemy))]
    [HarmonyPrefix]
    static bool DebugSpawnEnemyOverride(ref int ___enemyTypeId, ref SelectableLevel ___testAllEnemiesLevel, ref int ___enemyToSpawnId, ref int ___numberEnemyToSpawn, ref Transform[] ___debugEnemySpawnPositions)
    {
        Plugin.Instance.Debug_SpawnEnemy(___enemyTypeId, ___testAllEnemiesLevel, ___enemyToSpawnId, ___numberEnemyToSpawn, ___debugEnemySpawnPositions);

        return false;
    }

    [HarmonyPatch(nameof(QuickMenuManager.CanEnableDebugMenu))]
    [HarmonyPostfix]
    static bool DebugCanEnableDebugMenuOverride(bool canEnable, ref int ___enemyTypeId, ref SelectableLevel ___testAllEnemiesLevel, ref int ___enemyToSpawnId, ref int ___numberEnemyToSpawn, ref Transform[] ___debugEnemySpawnPositions)
    {
        return NetworkManager.Singleton.IsServer;
    }
}