using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PWS.TexturePainter;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Locations;

namespace PowerwashSimAP.Patches;

public static class WashTargetPatch
{
    public static HashSet<string> WashTargets = [];
    public static ConcurrentBag<WashTarget> GlobalTargets = [];
    
    [HarmonyPatch(typeof(WashTarget), "Start"), HarmonyPostfix]
    public static void Start(WashTarget __instance)
    {
        GlobalTargets.Add(__instance);
        
        if (Plugin.IsDebug is not Plugin.DebugWant.Washables) return;
        WashTargets.Add(__instance.gameObject.name.Replace("_", " "));
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

            if (IsMissing(LocationName))
            {
                enabled = false;
                return;
            }

            if (ChecksToSendQueue.Contains(LocationName)) return;
            ChecksToSendQueue.Enqueue(LocationName);
            enabled = false;
        }
    }
}