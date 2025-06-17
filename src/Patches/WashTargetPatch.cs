using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PWS.TexturePainter;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Patches.Locations;

namespace PowerwashSimAP.Patches;

public static class WashTargetPatch
{
    public static List<string> WashTargets = [];
    
    [HarmonyPatch(typeof(WashTarget), "Start"), HarmonyPostfix]
    public static void Start(WashTarget __instance)
    {
        __instance.gameObject.GetOrAddComponent<WashTargetUpdate>();
        if (Plugin.IsDebug is not (Plugin.DebugWant.Washables or Plugin.DebugWant.WashablesAndPrint)) return;
        WashTargets.Add(__instance.gameObject.name.Replace("_", " "));
        if (Plugin.IsDebug is not Plugin.DebugWant.WashablesAndPrint) return;
        Plugin.Log.LogInfo($"Added| [{__instance.gameObject.name}]");
    }

    public class WashTargetUpdate : MonoBehaviour
    {
        public string LocationName = "";
        public WashTarget Target;

        private void Start() => Target = gameObject.GetComponent<WashTarget>();

        private void Update()
        {
            if (!Objectsanity || Client is null) return;
            if (Target.CleanProgress < 1) return;

            if (LocationName == "")
            {
                LocationName = $"{SceneNameToLocationName[gameObject.scene.name]}: {gameObject.name.Replace("_", " ")}";
            }

            if (Client.MissingLocations.All(kv => kv.Value.LocationName != LocationName))
            {
                enabled = false;
                return;
            }

            var location = Client.MissingLocations.First(kv => kv.Value.LocationName == LocationName).Key;
            if (ChecksToSendQueue.Contains(location)) return;
            ChecksToSendQueue.Enqueue(location);
            enabled = false;
        }
    }
}