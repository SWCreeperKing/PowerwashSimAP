using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Text.RegularExpressions;
using PWS.Analytics;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Locations;
using static PowerwashSimAP.Patches.WashTargetPatch;

namespace PowerwashSimAP.Patches;

public static class LevelProgressionPatch
{
    public static string LocationName;
    public static int LastPercentChecked = 1;

    [HarmonyPatch(typeof(LevelProgressionSender), "Start"), HarmonyPostfix]
    public static void Init(LevelProgressionSender __instance)
    {
        foreach (var washTarget in GlobalTargets)
        {
            washTarget.gameObject.GetOrAddComponent<WashTargetUpdate>();
        }

        GlobalTargets = [];

        if (Plugin.IsDebug is Plugin.DebugWant.Washables)
        {
            CleanParts[__instance.gameObject.scene.name] = WashTargets.OrderBy(s => s).ToArray();
            WashTargets.Clear();
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

            if (LastPercentChecked == 100)
            {
                Plugin.Log.LogInfo($"Completed Level: [{LocationName}]");
                SetLevelCompletion(LocationName);
                FailsafeSendLocations(LocationName);
            }

            LastPercentChecked++;
            if (!IsMissing(percentName)) continue;
            ChecksToSendQueue.Enqueue(percentName);
        }
    }
}