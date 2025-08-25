using HarmonyLib;
using PWS;
using static PowerwashSimAP.Plugin;

namespace PowerwashSimAP.Patches;

public static class ShopPatch
{
     [HarmonyPatch(typeof(ShopScreen), "Start"), HarmonyPostfix]
     public static void Init(ShopScreen __instance)
     {
          var child = __instance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
          Log.LogInfo(child.name);

          foreach (var item in child.gameObject.GetChildren())
          {
               item.SetActive(false);
          }
     }
}