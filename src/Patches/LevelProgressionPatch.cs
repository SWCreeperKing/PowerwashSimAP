using System;
using System.Linq;
using HarmonyLib;
using PWS.Analytics;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;

namespace PowerwashSimAP.Patches;

public static class LevelProgressionPatch
{
    public static int LastSentPercentage;
    public static string LocationName;

    [HarmonyPatch(typeof(LevelProgressionSender), "Start"), HarmonyPostfix]
    public static void Init(LevelProgressionSender __instance)
    {
        Plugin.Log.LogInfo($"Scene name: [{__instance.gameObject.scene.name}]");
        LocationName = Locations.SceneNameToLocationName[__instance.gameObject.scene.name];
    }

    [HarmonyPatch(typeof(LevelProgressionSender), "HandleProgressChanged"), HarmonyPostfix]
    public static void Update(LevelProgressionSender __instance)
    {
        if (Client is null) return;
        var percentage = __instance.m_currentPercentage;
        if (percentage % 20 != 0 || LastSentPercentage == percentage) return;
        LastSentPercentage = percentage;
        var percentName = $"{LocationName} {percentage}%";

        if (Client.MissingLocations.All(kv => kv.Value.LocationName != percentName)) return;
        var location = Client.MissingLocations.First(kv => kv.Value.LocationName == percentName).Key;
        Client.SendLocation(location);
    }
}