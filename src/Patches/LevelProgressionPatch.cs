using System;
using System.Collections.Generic;
using System.Linq;
using FuturLab;
using HarmonyLib;
using PWS;
using PWS.Analytics;
using PWS.TexturePainter;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using Object = UnityEngine.Object;

namespace PowerwashSimAP.Patches;

public static class LevelProgressionPatch
{
    public static string LocationName;
    public static int LastPercentChecked = 1;
    public static Dictionary<string, WashTarget> WashTargets = [];
    public static List<string> Cleaned = [];

    [HarmonyPatch(typeof(LevelProgressionSender), "Start"), HarmonyPostfix]
    public static void Init(LevelProgressionSender __instance)
    {
        // Plugin.Log.LogInfo($"Scene name: [{__instance.gameObject.scene.name}]");
        WashTargets.Clear();
        Cleaned.Clear();
        LocationName = Locations.SceneNameToLocationName[__instance.gameObject.scene.name];
        LastPercentChecked = 1;

        var washTargetRoot = Object.FindObjectOfType<WashTargetRoot>();
        foreach (var washTarget in washTargetRoot.m_washTargets)
        {
            var objName = washTarget.name.Replace("_", " ");
            WashTargets[objName] = washTarget;
        }

        // GUIUtility.systemCopyBuffer = $"[{string.Join(", ", WashTargets.Keys.Select(s => $"\"{s}\""))}]";
        // Plugin.Log.LogInfo("Copied");
    }

    [HarmonyPatch(typeof(LevelProgressionSender), "HandleProgressChanged"), HarmonyPostfix]
    public static void Update(LevelProgressionSender __instance)
    {
        if (Client is null) return;

        if ((bool)Client.SlotData["objectsanity"])
        {
            foreach (var washKv in WashTargets)
            {
                if (Cleaned.Contains(washKv.Key)) continue;
                if (washKv.Value.CleanProgress < 1) continue;
                var locationName = $"{LocationName}: {washKv.Key}";
                if (Client.MissingLocations.All(kv => kv.Value.LocationName != locationName)) return;
                var location = Client.MissingLocations.First(kv => kv.Value.LocationName == locationName).Key;
                Client.SendLocation(location);
                Cleaned.Add(washKv.Key);
            }
        }

        if (!(bool)Client.SlotData["percentsanity"]) return;
        var percentage = __instance.m_currentPercentage;
        while (LastPercentChecked <= percentage)
        {
            var percentName = $"{LocationName} {LastPercentChecked}%";
            LastPercentChecked++;

            if (Client.MissingLocations.All(kv => kv.Value.LocationName != percentName)) return;
            var location = Client.MissingLocations.First(kv => kv.Value.LocationName == percentName).Key;
            Client.SendLocation(location);
        }
    }
}