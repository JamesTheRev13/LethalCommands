
using HarmonyLib;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
public class TimeOfDayPatches
{
    [HarmonyPatch(nameof(TimeOfDay.MoveGlobalTime))]
    [HarmonyPostfix]
    static void InfiniteDeadline(ref float ___timeUntilDeadline)
    {
        if (Plugin.Instance.infiniteDeadline) { ___timeUntilDeadline = 5000; }

    }
}