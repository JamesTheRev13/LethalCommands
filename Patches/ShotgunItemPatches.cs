
using HarmonyLib;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(ShotgunItem))]
public class ShotgunItemPatches
{
    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    [HarmonyPostfix]
    static void ammoOverride(ref int ___shellsLoaded)
    {
        if (Plugin.Instance.infiniteAmmo)
            ___shellsLoaded = 2147483647;
    }
}