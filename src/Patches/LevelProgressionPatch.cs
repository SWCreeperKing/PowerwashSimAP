using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreepyUtil.Archipelago;
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
        UpdateChecks(__instance);
    }

    [HarmonyPatch(typeof(LevelProgressionSender), "HandleProgressChanged"), HarmonyPostfix]
    public static void ProgressChanged(LevelProgressionSender __instance) => UpdateChecks(__instance);

    public static void UpdateChecks(LevelProgressionSender __instance)
    {
        if (Client is null) return;

        if (Objectsanity)
        {
            foreach (var washKv in WashTargets)
            {
                if (Cleaned.Contains(washKv.Key)) continue;
                if (washKv.Value.CleanProgress < 1) continue;
                var locationName = $"{LocationName}: {washKv.Key}";
                if (Client.MissingLocations.All(kv => kv.Value.LocationName != locationName)) continue;
                var location = Client.MissingLocations.First(kv => kv.Value.LocationName == locationName).Key;
                ChecksToSend.Add(location);
                Cleaned.Add(washKv.Key);
            }
        }

        if (Percentsanity)
        {
            var percentage = __instance.m_currentPercentage;
            while (LastPercentChecked <= percentage)
            {
                var percentName = $"{LocationName} {LastPercentChecked}%";
                LastPercentChecked++;

                if (Client.MissingLocations.All(kv => kv.Value.LocationName != percentName)) continue;
                var location = Client.MissingLocations.First(kv => kv.Value.LocationName == percentName).Key;
                ChecksToSend.Add(location);
            }
        }
    }
}