using System;
using System.Collections.Generic;
using System.Linq;
using FuturLab;
using HarmonyLib;
using ProceduralWorlds.SceneOptimizer;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Patches.Locations;

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

    public static Dictionary<string, string[]> Lookahead = new()
    {
        // base game
        ["MainMenuButton_CareerOverview"] = ["CareerOverviewScreen(Clone)", "MainMenuButton_Specials"],
        ["CareerOverviewScreen(Clone)"] = ["LocationsButton", "VehiclesButton"],
        ["VehiclesButton"] = RawLocationData.Take(18).Select(arr => arr[0]).ToArray(),
        ["FilterButton(Clone) (0)"] = RawLocationData.Take(12).Select(arr => arr[0]).ToArray(),
        ["FilterButton(Clone) (1)"] = RawLocationData.Skip(12).Take(2).Select(arr => arr[0]).ToArray(),
        ["FilterButton(Clone) (2)"] = RawLocationData.Skip(14).Take(4).Select(arr => arr[0]).ToArray(),
        ["LocationsButton"] = RawLocationData.Skip(18).Take(20).Select(arr => arr[0]).ToArray(),

        // bonus jobs
        ["MainMenuButton_Specials"] =
        [
            "BonusGridElement(Clone) (0)", "BonusGridElement(Clone) (1)", "BonusGridElement(Clone) (2)",
            "BonusGridElement(Clone) (3)", "BonusGridElement(Clone) (4)", "BonusGridElement(Clone) (5)",
            "BonusGridElement(Clone) (6)"
        ],
        ["BonusGridElement(Clone) (0)"] =
            ["MARS_MARSROVER", "RECREATIONGROUND_FOUNTAIN", "RECREATIONGROUND_MINIGOLF", "DESERT_STEAMLOCOMOTIVE"],
        ["BonusGridElement(Clone) (1)"] = ["RECREATIONGROUND_FOODTRUCK", "DESERT_SATELLITEDISH", "DESERT_SOLAR"],
        ["BonusGridElement(Clone) (2)"] =
            ["RECREATIONGROUND_PAINTBALL", "RESIDENTIALSMALL_SPANISHVILLA", "RESIDENTIALSMALL_EXCAVATOR"],
        ["BonusGridElement(Clone) (3)"] = ["AQUARIUM", "SUBMARINE"],
        ["BonusGridElement(Clone) (4)"] = ["MODERNMANSION", "FIREPLANE"],
        ["BonusGridElement(Clone) (5)"] = ["DESSERTPARLOUR"],
        ["BonusGridElement(Clone) (6)"] = ["SUBWAYTRAIN", "SCULPTUREPARK"],

        // dlc
        ["MainMenuButton_DLC"] = ["DLCGridElement(Clone) (6)", "DLCGridElement(Clone) (7)"],
        ["DLCGridElement(Clone) (6)"] = ["FinalFantasyCampaignScreen(Clone)"],
        ["FinalFantasyCampaignScreen(Clone)"] =
        [
            "WAREHOUSE_SENTINEL", "FINALFANTASY_SHINRASHOWROOM", "FINALFANTASY_SEVENTHHEAVEN",
            "FINALFANTASY_MIDGARDIORAMA", "WAREHOUSE_AIRBUSTER"
        ],
        ["DLCGridElement(Clone) (7)"] = ["TombRaiderCampaignScreen(Clone)"],
        ["TombRaiderCampaignScreen(Clone)"] =
        [
            "TOMBRAIDER_MANOR", "TOMBRAIDER_OBSTACLECOURSE", "TOMBRAIDER_VEHICLES", "TOMBRAIDER_MAZE",
            "TOMBRAIDER_TREASUREROOM"
        ],
    };

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
        if (Allowed.Contains(locationName)) return IsMissing(LabelNameToLocationName[locationName]);
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