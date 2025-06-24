using HarmonyLib;
using PWS;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Patches.Locations;

namespace PowerwashSimAP.Patches;

public static class JobLevelPatch
{
    [HarmonyPatch(typeof(JobListElement), "Start"), HarmonyPostfix]
    public static void Init(JobListElement __instance)
    {
        var sceneName = __instance.Content.UniqueName;
        if (Plugin.IsDebug is Plugin.DebugWant.Jobs)
        {
            Plugin.Log.LogInfo(sceneName);
        }
        
        if (Client is null) return;
        if (Plugin.IsDebug is not Plugin.DebugWant.None) return;
        __instance.gameObject.SetActive(Allowed.Contains(sceneName) && IsMissing(LabelNameToLocationName[sceneName]));
    }
}