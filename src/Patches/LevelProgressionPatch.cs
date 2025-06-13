using System;
using HarmonyLib;
using PWS.Analytics;
using UnityEngine;

namespace PowerwashSimAP.Patches;

public static class LevelProgressionPatch
{
    public static int LastSentPercentage;
    
    [HarmonyPatch(typeof(LevelProgressionSender), "Start"), HarmonyPostfix]
    public static void Init(LevelProgressionSender __instance)
    {
        Component component = __instance;
        Plugin.Log.LogInfo($"Scene name: [{component.gameObject.scene.name}]");
        
        var initPercent = __instance.m_currentPercentage;
        LastSentPercentage = (int)(Math.Floor(initPercent/5f) * 5);
        Plugin.Log.LogInfo($"Scene source: [{__instance}]");
        Plugin.Log.LogInfo($"init percent: [{initPercent}], 5% milestone: [{LastSentPercentage}]");
    }
    
    [HarmonyPatch(typeof(LevelProgressionSender), "HandleProgressChanged"), HarmonyPostfix]
    public static void Update(LevelProgressionSender __instance)
    {
        var percentage = __instance.m_currentPercentage;
        
        if (percentage % 5 != 0 || LastSentPercentage == percentage) return;
        LastSentPercentage = percentage;
        Plugin.Log.LogInfo($"Percentage: {percentage}");
    }
}