using System.Linq;
using FuturLab;
using HarmonyLib;
using PWS;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PowerwashSimAP.Patches;

public static class BuyPowerWasherPatch
{
    [HarmonyPatch(typeof(GridElementPowerWasher), "Start"), HarmonyPostfix]
    public static void PowerwasherGridElement(GridElementPowerWasher __instance)
    {
        var children = __instance.gameObject.GetChildren();
        Plugin.Log.LogInfo($"[{children.Length}]: [{string.Join(", ", children.Select(child => child.name))}]");
        var topChildren = children[1].GetChildren();
        var price = topChildren[0].GetComponent<TextMeshProUGUI>();
        var name = topChildren[1].GetComponent<TextMeshProUGUI>();
        var buyGObj = topChildren[2];
        topChildren[4].SetActive(false);
        topChildren[6].SetActive(false);

        price.text = "$6,969.69";
        name.text = "Immortality";

        var rawBuyButtonAnimator = buyGObj.GetComponent<FuturAnimatedButton>();
        var rawBuyButton = buyGObj.GetComponent<FuturButton>();

        rawBuyButton.enabled = false;
        rawBuyButtonAnimator.enabled = false;

        // var buyButton = rawBuyButton.Button;
        // var onClick = buyButton.onClick;
        // onClick.RemoveAllListeners();
        // onClick.;
    }
}