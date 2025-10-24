using BepInEx.Logging;
using PWS;
using HarmonyLib;
using PlayFab.Internal;
using UnityEngine;

namespace PowerwashSimAP.Patches;

public static class HasJobBeenPlayedPatch
{
    [HarmonyPatch(typeof(SaveManager), "HasJobBeenPlayed"), HarmonyPrefix]
    public static bool Prefix(ref bool __result)
    {
        Plugin.Log.LogInfo($"HasJobBeenPlayedPatch::Prefix called, patching function");
        // Pretend every job has been played
        __result = true;

        // Skip original method
        return false;
    }
}
