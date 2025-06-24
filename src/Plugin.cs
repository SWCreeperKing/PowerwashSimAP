using System;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using PowerwashSimAP.Patches;
using UnhollowerRuntimeLib;
using UnityEngine;
using static PowerwashSimAP.Patches.Locations;

namespace PowerwashSimAP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public enum DebugWant
    {
        None,
        Stats,
        Buttons,
        Jobs,
        Washables,
        WashablesAndPrint,
        TranslateWashables,
        Failsafe
    }

    public static DebugWant IsDebug = DebugWant.None;
    public new static ManualLogSource Log;

    public static event EventHandler<Plugin> Unloaded;

    public override void Load()
    {
        Log = base.Log;

        ClassInjector.RegisterTypeInIl2Cpp<APGui>();
        ClassInjector.RegisterTypeInIl2Cpp<MainMenuButtonPatch.AlwaysInvisible>();
        ClassInjector.RegisterTypeInIl2Cpp<MainMenuButtonPatch.VisibleControlComponent>();
        ClassInjector.RegisterTypeInIl2Cpp<WashTargetPatch.WashTargetUpdate>();
        Harmony.CreateAndPatchAll(typeof(LevelProgressionPatch));
        Harmony.CreateAndPatchAll(typeof(JobLevelPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuButtonPatch));
        Harmony.CreateAndPatchAll(typeof(WashTargetPatch));

        if (IsDebug is DebugWant.TranslateWashables)
        {
            StringBuilder sb = new();
            sb.Append("raw_objectsanity_dict = {\n");

            foreach (var kv in CleanParts)
            {
                sb.Append(
                    $"\t\"{SceneNameToLocationName[kv.Key]}\": [{string.Join(", ", kv.Value.Select(s => $"\"{s}\""))}],\n");
            }

            sb.Append("}");
            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        if (IsDebug is DebugWant.Stats)
        {
            Log.LogInfo(
                $"\n{string.Join("\n", CleanParts.OrderBy(kv => kv.Value.Length).Select(kv => $"{SceneNameToLocationName[kv.Key]} has [{kv.Value.Length}] parts"))}");
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