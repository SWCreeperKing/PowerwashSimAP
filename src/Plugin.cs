using System;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using PowerwashSimAP.Patches;
using UnhollowerRuntimeLib;

namespace PowerwashSimAP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public new static ManualLogSource Log;
    
    public static event EventHandler<Plugin> Unloaded;

    public override void Load()
    {
        Log = base.Log;

        ClassInjector.RegisterTypeInIl2Cpp<APGui>();
        Harmony.CreateAndPatchAll(typeof(LevelProgressionPatch));
        Harmony.CreateAndPatchAll(typeof(JobLevelPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuPatch));
        Harmony.CreateAndPatchAll(typeof(MainMenuButtonPatch));
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    public override bool Unload()
    {
        Unloaded?.Invoke(this, this);
        Log.LogInfo($"Plugin [{MyPluginInfo.PLUGIN_GUID}] has unloaded!");
        return true;
    }
}