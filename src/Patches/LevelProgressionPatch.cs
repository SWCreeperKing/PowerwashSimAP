using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PWS;
using PWS.Analytics;
using PWS.TexturePainter;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Patches.Locations;

namespace PowerwashSimAP.Patches;

public static class LevelProgressionPatch
{
    public static string LocationName;
    public static int LastPercentChecked = 1;

    [HarmonyPatch(typeof(LevelProgressionSender), "Start"), HarmonyPostfix]
    public static void Init(LevelProgressionSender __instance)
    {
        if (Plugin.IsDebug is Plugin.DebugWant.Washables or Plugin.DebugWant.WashablesAndPrint)
        {
            GUIUtility.systemCopyBuffer =
                $"[\"{__instance.gameObject.scene.name}\"] = [{string.Join(", ", WashTargetPatch.WashTargets.Select(s => $"\"{s}\""))}]";
            if (Plugin.IsDebug is Plugin.DebugWant.WashablesAndPrint)
            {
                Plugin.Log.LogInfo("Copied");
            }

            WashTargetPatch.WashTargets.Clear();
            return;
        }

        LocationName = SceneNameToLocationName[__instance.gameObject.scene.name];
        LastPercentChecked = 1;
        ProgressChanged(__instance);
    }

    [HarmonyPatch(typeof(LevelProgressionSender), "HandleProgressChanged"), HarmonyPostfix]
    public static void ProgressChanged(LevelProgressionSender __instance)
    {
        if (Client is null || !Percentsanity) return;

        var percentage = __instance.m_currentPercentage;
        while (LastPercentChecked <= percentage)
        {
            var percentName = $"{LocationName} {LastPercentChecked}%";

            // if (LastPercentChecked == 100) // testing failsafe
            // {
            //     foreach (var loc in Client.MissingLocations.Where(kv
            //                  => kv.Value.LocationName.StartsWith(LocationName)))
            //     {
            //         if (ChecksToSendQueue.Contains(loc.Key)) continue;
            //         ChecksToSendQueue.Enqueue(loc.Key);
            //     }
            // }

            LastPercentChecked++;
            if (Client.MissingLocations.All(kv => kv.Value.LocationName != percentName)) continue;
            var location = Client.MissingLocations.First(kv => kv.Value.LocationName == percentName).Key;
            if (!ChecksToSendQueue.Contains(location))
            {
                ChecksToSendQueue.Enqueue(location);
            }
        }
    }
}