
using HarmonyLib;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatches
{
    [HarmonyPatch(nameof(Terminal.RunTerminalEvents))]
    [HarmonyPostfix]
    static void InfiniteCredits(ref int ___groupCredits)
    {
        if (Plugin.Instance.infiniteCredits) { ___groupCredits = 69420; }
    }
}