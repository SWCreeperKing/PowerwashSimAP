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

public static class FreePlayUnlockPatch
{
    [HarmonyPatch(typeof(CampaignSaveData), "IsFreePlayUnlocked"), HarmonyPrefix]
    static bool IsFreePlayUnlocked_Prefix(ref bool __result)
    {
        __result = true;  // All jobs unlocked, period
        return false;     // Skip original method entirely
    }
}

public static class UnlockAllEquipmentPatch
{
    [HarmonyPatch(typeof(GameStateManager), "IsEquipmentUnlocked", typeof(BaseEquipmentData)), HarmonyPrefix]
    public static bool IsEquipmentUnlocked_Prefix(ref bool __result)
    {
        // Plugin.Log.LogInfo("IsEquipmentUnlocked patch: returning true for all equipment");
        __result = true;
        return false;
    }
}
