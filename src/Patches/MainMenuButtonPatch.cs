using System.Collections.Generic;
using System.Linq;
using FuturLab;
using HarmonyLib;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Locations;

namespace PowerwashSimAP.Patches;

public static class MainMenuButtonPatch
{
    public static string[] ButtonBlacklist =
    [
        "MainMenuButton_Career", "MainMenuButton_Challenges", "CategoryNavigateLeftButton",
        "MainMenuButton_DLC_Shop(Clone)", "CategoryNavigateRightButton",
        "MainMenuButton_Bonus_Shop(Clone)", "MainMenuButton_SeasonalSpecials(Clone)",
        "DLCGridElement(Clone) (0)",
        "DLCGridElement(Clone) (1)",
        "DLCGridElement(Clone) (2)",
        "DLCGridElement(Clone) (3)",
        "DLCGridElement(Clone) (4)",
        "DLCGridElement(Clone) (5)",
    ];

    public static Dictionary<string, string[]> Lookahead;

    [HarmonyPatch(typeof(FuturButton), "Start"), HarmonyPostfix]
    public static void Start(FuturButton __instance)
    {
        var obj = __instance.gameObject;
        var objName = obj.GetProperName();

        if (Plugin.IsDebug is not Plugin.DebugWant.None)
        {
            if (Plugin.IsDebug is Plugin.DebugWant.Buttons)
            {
                Plugin.Log.LogInfo($"Button Name: {objName}");
            }

            return;
        }

        if (Lookahead.ContainsKey(objName))
        {
            var visibleController = obj.GetParent().AddComponent<VisibleControlComponent>();
            visibleController.ObjName = objName;
            visibleController.Obj = obj;
            return;
        }

        HideBlacklist(objName, obj);
    }

    public static void HideBlacklist(string objName, GameObject obj)
    {
        if (!ButtonBlacklist.Contains(objName)) return;
        obj.SetActive(false);
        obj.AddComponent<AlwaysInvisible>();
    }

    public static bool IsAnyChildVisible(string locationName)
    {
        if (Client is null) return false;
        if (Allowed.Contains(locationName)) return IsMissingStartsWith(LabelNameToLocationName[locationName]);
        return Lookahead.TryGetValue(locationName, out var nextUp) && nextUp.Any(IsAnyChildVisible);
    }

    public static GameObject GetParent(this GameObject go) => go.transform.parent.gameObject;

    public static string GetProperName(this GameObject obj)
    {
        var objName = obj.name;
        objName = objName == "BonusGridElement" ? "BonusGridElement(Clone)" : objName;
        objName = objName == "DLCGridElement" ? "DLCGridElement(Clone)" : objName;
        return objName switch
        {
            "MainMenuButton_FreePlay" => obj.GetParent().GetParent().GetParent().GetParent().GetParent().name,
            "FilterButton(Clone)" or "BonusGridElement(Clone)" or "DLCGridElement(Clone)" =>
                $"{objName} ({obj.transform.GetSiblingIndex()})",
            _ => objName
        };
    }

    public class VisibleControlComponent : MonoBehaviour
    {
        public string ObjName;
        public GameObject Obj;

        private void Update() => Obj.SetActive(IsAnyChildVisible(ObjName));
    }

    public class AlwaysInvisible : MonoBehaviour
    {
        private void Update() => gameObject.SetActive(false);
    }
}