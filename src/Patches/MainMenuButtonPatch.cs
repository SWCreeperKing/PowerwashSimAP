using System.Linq;
using FuturLab;
using HarmonyLib;

namespace PowerwashSimAP.Patches;

public static class MainMenuButtonPatch
{
    public static string[] ButtonBlacklist =
    [
        "MainMenuButton_Career", "MainMenuButton_Specials", "MainMenuButton_Challenges", "MainMenuButton_DLC",
        "CategoryNavigateLeftButton", "CategoryNavigateRightButton"
    ];

    [HarmonyPatch(typeof(FuturButton), "Start"), HarmonyPostfix]
    public static void Start(FuturButton __instance)
    {
        Plugin.Log.LogInfo($"{__instance.gameObject.name} | [{ButtonBlacklist.Contains(__instance.gameObject.name)}]");
        if (!ButtonBlacklist.Contains(__instance.gameObject.name)) return;
        __instance.gameObject.SetActive(false);

        try
        {
            __instance.gameObject.GetComponent<FuturAnimatedButton>().enabled = false;
        }
        catch
        {
            //ignore
        }
    }
}