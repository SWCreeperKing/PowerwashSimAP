using System.Collections.Generic;
using HarmonyLib;
using PWS;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Patches.Locations;

namespace PowerwashSimAP.Patches;

public static class JobLevelPatch
{
    public static List<string> Allowed = [];

    [HarmonyPatch(typeof(JobListElement), "Start"), HarmonyPostfix]
    public static void Init(JobListElement __instance)
    {
        // if (Client is null) return;
        var sceneName = __instance.Content.UniqueName;
        __instance.gameObject.SetActive(Allowed.Contains(sceneName) && IsMissing(LabelNameToLocationName[sceneName]));
    }
}