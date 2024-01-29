
using HarmonyLib;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(GrabbableObject))]
public class GrabbableObjectPatches
{
    [HarmonyPatch(nameof(GrabbableObject.Update))]
    [HarmonyPrefix]
    static void batteryOverride(ref Battery ___insertedBattery)
    {
        if (Plugin.Instance.infiniteBattery)
        {
            ___insertedBattery.charge = 100.0f;
            ___insertedBattery.empty = false;
        }
    }
}