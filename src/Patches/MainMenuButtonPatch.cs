using FuturLab;
using HarmonyLib;

namespace PowerwashSimAP.Patches;

public static class MainMenuButtonPatch
{
    [HarmonyPatch(typeof(FuturButton), "Start"), HarmonyPostfix]
    public static void Start(FuturButton __instance)
    {
        if (__instance.gameObject.name is not ("MainMenuButton_Career" or "MainMenuButton_Specials"
            or "MainMenuButton_Challenges" or "MainMenuButton_DLC")) return;
        __instance.gameObject.SetActive(false);
    }
}