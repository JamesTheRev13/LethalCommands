
using HarmonyLib;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManagerPatches
{
    //[HarmonyPatch(nameof(RoundManager.EnemyCannotBeSpawned))]
    //[HarmonyPrefix]
    //// Ignores if spawning is disabled, enemy power levels, and maxCount
    //static bool OverrideEnemySpawn()
    //{
    //    // Should add this behind a toggle... may cause some unintended funky behavior
    //    ManualLogSource.LogInfo("Force Spawned Enemy -> EnemyCannotBeSpawned: false");
    //    return false;
    //}
}