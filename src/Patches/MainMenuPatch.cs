using HarmonyLib;
using PWS;
using UnityEngine;

namespace PowerwashSimAP.Patches;

public static class MainMenuPatch
{
    [HarmonyPatch(typeof(PlayerUIInput), "Start"), HarmonyPostfix]
    public static void Start(PlayerUIInput __instance)
    {
        if (__instance.gameObject.scene.name != "DontDestroyOnLoad") return;

        var obj = __instance.gameObject;
        APGui.Offset = new Vector2(15, 250);
        obj.AddComponent<APGui>();
    }
    
    public static GameObject GetObject(string name) => GameObject.Find(name);
    public static void FindAndHide(string name) => GetObject(name)?.SetActive(false);
}