using System;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using PowerwashSimAP.Patches;
using UnhollowerRuntimeLib;
using static PowerwashSimAP.Locations;

namespace PowerwashSimAP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public const string ModDir = "BepInEx/plugins/SW_CreeperKing.ArchipelagoMod";

    public enum DebugWant
    {
        None = 0,
        General = 1,
        Stats = 2,
        Buttons = 3,
        Jobs = 4,
        Washables = 5,
        TranslateWashables = 6,
    }

    public static DebugWant IsDebug = DebugWant.None;
    public new static ManualLogSource Log;

    public static event EventHandler<Plugin> Unloaded;

    public override void Load()
    {
        if (File.Exists("debug.txt"))
        {
            if (int.TryParse(File.ReadAllText("debug.txt"), out var isDebug))
            {
                IsDebug = (DebugWant)isDebug;
            }
        }

        Log = base.Log;
        Log.LogInfo($"Debug Setting: [{IsDebug}]");

        if (!File.Exists($"{ModDir}/Locations.txt"))
        {
            Log.LogError("NO LOCATIONS FOUND, EXITING LOAD");
            return;
        }

        if (!File.Exists($"{ModDir}/LocationTransformData.txt"))
        {
            Log.LogError("NO LOCATION TRANSFORMS FOUND, EXITING LOAD");
            return;
        }

        RawLocationData = File.ReadAllText($"{ModDir}/LocationTransformData.txt")
                              .Replace("\r", "")
                              .Split('\n')
                              .Where(line => !line.StartsWith("//") && line != "")
                              .Select(line => line.Split(','))
                              .ToArray();

        CleanParts = File.ReadAllText($"{ModDir}/Locations.txt")
                         .Replace("\r", "")
                         .Split('\n')
                         .Select(line => line.Split(':'))
                         .ToArray()
                         .ToDictionary(arr => arr[0], arr => arr[1].Split(','));

        LevelUnlockDictionary = RawLocationData.ToDictionary(arr => $"{arr[1]} Unlock", arr => arr[0]);
        LevelDictionary = RawLocationData.ToDictionary(arr => arr[1], arr => arr[0]);
        SceneNameToLocationName = RawLocationData.ToDictionary(arr => arr[2], arr => arr[1]);
        LabelNameToLocationName = RawLocationData.ToDictionary(arr => arr[0], arr => arr[1]);

        MainMenuButtonPatch.Lookahead = new()
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
            ["BonusGridElement(Clone) (0)"] = RawLocationData.Skip(38).Take(4).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (1)"] = RawLocationData.Skip(42).Take(3).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (2)"] = RawLocationData.Skip(45).Take(3).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (3)"] = RawLocationData.Skip(48).Take(2).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (4)"] = RawLocationData.Skip(50).Take(2).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (5)"] = RawLocationData.Skip(52).Take(1).Select(arr => arr[0]).ToArray(),
            ["BonusGridElement(Clone) (6)"] = RawLocationData.Skip(53).Take(2).Select(arr => arr[0]).ToArray(),

            // dlc
            ["MainMenuButton_DLC"] =
            [
                "DLCGridElement(Clone) (0)", "DLCGridElement(Clone) (1)", "DLCGridElement(Clone) (2)",
                "DLCGridElement(Clone) (3)", "DLCGridElement(Clone) (4)", "DLCGridElement(Clone) (5)",
                "DLCGridElement(Clone) (6)", "DLCGridElement(Clone) (7)"
            ],
            ["DLCGridElement(Clone) (6)"] = ["FinalFantasyCampaignScreen(Clone)"],
            ["FinalFantasyCampaignScreen(Clone)"] = RawLocationData.Skip(55).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (7)"] = ["TombRaiderCampaignScreen(Clone)"],
            ["TombRaiderCampaignScreen(Clone)"] = RawLocationData.Skip(60).Take(5).Select(arr => arr[0]).ToArray(),

            // paid dlc
            ["DLCGridElement(Clone) (0)"] = ["CheeseCampaignScreen(Clone)"],
            ["CheeseCampaignScreen(Clone)"] = RawLocationData.Skip(65).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (1)"] = ["ShrekCampaignScreen(Clone)"],
            ["ShrekCampaignScreen(Clone)"] = RawLocationData.Skip(70).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (2)"] = ["AAPCampaignScreen(Clone)"],
            ["AAPCampaignScreen(Clone)"] = RawLocationData.Skip(75).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (3)"] = ["Warhammer40KCampaignScreen(Clone)"],
            ["Warhammer40KCampaignScreen(Clone)"] = RawLocationData.Skip(80).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (4)"] = ["BTTFCampaignScreen(Clone)"],
            ["BTTFCampaignScreen(Clone)"] = RawLocationData.Skip(85).Take(5).Select(arr => arr[0]).ToArray(),
            ["DLCGridElement(Clone) (5)"] = ["SpongeBobCampaignScreen(Clone)"],
            ["SpongeBobCampaignScreen(Clone)"] = RawLocationData.Skip(90).Take(6).Select(arr => arr[0]).ToArray(),
        };

        ClassInjector.RegisterTypeInIl2Cpp<APGui>();
        ClassInjector.RegisterTypeInIl2Cpp<MainMenuButtonPatch.AlwaysInvisible>();
        ClassInjector.RegisterTypeInIl2Cpp<MainMenuButtonPatch.VisibleControlComponent>();
        ClassInjector.RegisterTypeInIl2Cpp<WashTargetPatch.WashTargetUpdate>();
        Harmony.CreateAndPatchAll(typeof(LevelProgressionPatch));
        Harmony.CreateAndPatchAll(typeof(JobLevelPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuButtonPatch));
        Harmony.CreateAndPatchAll(typeof(WashTargetPatch));
        // Harmony.CreateAndPatchAll(typeof(ShopPatch));
        // Harmony.CreateAndPatchAll(typeof(BuyPowerWasherPatch));

        if (IsDebug is DebugWant.Stats)
        {
            // Log.LogInfo(
            //     $"\n{string.Join("\n", CleanParts.OrderBy(kv => kv.Value.Length).Select(kv => $"{SceneNameToLocationName[kv.Key]} has [{kv.Value.Length}] parts"))}");
            // Log.LogInfo(
            //     $"\n{string.Join("\n", CleanParts.OrderBy(kv => kv.Value.Length).Select(kv => $"{kv.Key} has [{kv.Value.Length}] parts"))}");
            // Log.LogInfo($"Predicted total checks: [{(CleanParts.Sum(kv => 100 + kv.Value.Length)):###,###}]");

            var pairs = CleanParts.OrderBy(kv => kv.Value.Length)
                                  .Select(kv => (SceneNameToLocationName[kv.Key], kv.Value.Length))
                                  .ToArray();
            StringBuilder sb = new();
            sb.Append("| Level Name | Part Count |\n");
            sb.Append("|:-:|:-:|\n");
            sb.Append(string.Join("\n", pairs.Select(pair => $"| {pair.Item1} | {pair.Length} |")));
            sb.Append($"\n> Predicted total checks: [{CleanParts.Sum(kv => 100 + kv.Value.Length):###,###}]");
            File.WriteAllText("stats.md", sb.ToString());
        }

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        Unloaded?.Invoke(this, this);
        Log.LogInfo($"Plugin [{MyPluginInfo.PLUGIN_GUID}] has unloaded!");
        return true;
    }
}