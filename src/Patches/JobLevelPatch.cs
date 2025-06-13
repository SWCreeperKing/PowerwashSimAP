using System.Linq;
using HarmonyLib;
using PWS;

namespace PowerwashSimAP.Patches;

public static class JobLevelPatch
{
    public static string[] Allowed = ["HOME_VAN", "HOME_SUV"];
    
    [HarmonyPatch(typeof(JobListElement), "Start"), HarmonyPrefix]
    public static void Init(JobListElement __instance)
    {
        __instance.gameObject.SetActive(Allowed.Contains(__instance.Content.UniqueName));
    }
}