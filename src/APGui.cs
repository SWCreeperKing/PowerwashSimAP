using System.IO;
using System.Linq;
using Il2CppSystem;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;
using static PowerwashSimAP.Locations;

namespace PowerwashSimAP;

// stolen from: https://github.com/FyreDay/TCG-CardShop-Sim-APClient/blob/master/APGui.cs
public class APGui : MonoBehaviour
{
    public static bool ShowGUI = true;
    public static string Ipporttext = "archipelago.gg:12345";
    public static string Password = "";
    public static string Slot = "Powerwasher";
    public static string State = "";
    public static Vector2 Offset = Vector2.zero;
    private static double TimeAccumulator;

    public static GUIStyle TextStyle = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.white
        }
    };

    public static GUIStyle TextStyleGreen = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.green
        }
    };

    public static GUIStyle TextStyleRed = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.red
        }
    };

    private void Awake()
    {
        if (!File.Exists("ApConnection.txt")) return;
        var fileText = File.ReadAllText("ApConnection.txt").Replace("\r", "").Split('\n');
        Ipporttext = fileText[0];
        Password = fileText[1];
        Slot = fileText[2];
    }

    void OnGUI()
    {
        TimeAccumulator += Time.deltaTime;
        if (Plugin.IsDebug is Plugin.DebugWant.Washables)
        {
            if (GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 300, 30), "Save Washables"))
            {
                File.WriteAllText($"{Plugin.ModDir}/Locations.txt",
                    string.Join("\n", CleanParts.Select(kv => $"{kv.Key}:{string.Join(",", kv.Value)}")));

                File.WriteAllText("ApworldLocations.py",
                    $"raw_objectsanity_dict = {{\n{string.Join("\n", CleanParts.Select(kv => $"\t\"{SceneNameToLocationName[kv.Key]}\": [{string.Join(",", kv.Value.Select(v => $"\"{v}\""))}],"))}\n}}");

                File.WriteAllText("RawLocationNames.txt",
                    string.Join("\n", CleanParts.Select(kv => SceneNameToLocationName[kv.Key])));
            }

            return;
        }

        if (!ShowGUI) return;

        // Create a GUI window

        if (!IsConnected())
        {
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y, 200, 300), "AP Client");

            GUI.Label(new Rect(20 + Offset.x, 40 + Offset.y, 300, 30), "Address:port", TextStyle);
            Ipporttext = GUI.TextField(new Rect(20 + Offset.x, 60 + Offset.y, 180, 25), Ipporttext, 25);

            GUI.Label(new Rect(20 + Offset.x, 90 + Offset.y, 300, 30), "Password", TextStyle);
            Password = GUI.TextField(new Rect(20 + Offset.x, 110 + Offset.y, 180, 25), Password, 25);

            GUI.Label(new Rect(20 + Offset.x, 140 + Offset.y, 300, 30), "Slot", TextStyle);
            Slot = GUI.TextField(new Rect(20 + Offset.x, 160 + Offset.y, 180, 25), Slot, 25);
        }
        else
        {
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y + 100, 200, 150), "AP Client");
            if (Goal is GoalType.McGuffinHunt)
            {
                var hasGoal = Jobs >= WinCondition;
                GUI.Label(new Rect(20 + Offset.x, Offset.y + 155, 150, 35),
                    $"[A Job Well Done] mcguffins:\n    {Jobs} / {WinCondition}",
                    hasGoal ? TextStyleGreen : TextStyleRed);
            }
            else if (GoalLevelsOpen.Any())
            {
                var sixtySecondBarrier = Math.Floor(TimeAccumulator / 30);
                var index = (int)(sixtySecondBarrier % GoalLevelsOpen.Length);

                GUI.Label(new Rect(20 + Offset.x, Offset.y + 150, 150, 35),
                    $"""
                     Req. Completed Levels: ({CompletedLevelCount}/{LevelCount})
                     Levels of interest ({index + 1}/{GoalLevelsOpen.Length}):
                     [{GoalLevelsOpen[index]}]
                     """, TextStyle);
            }
        }

        if (!IsConnected() && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Connect"))
        {
            var ipPortSplit = Ipporttext.Split(':');
            if (!int.TryParse(ipPortSplit[1], out var port))
            {
                State = $"[{ipPortSplit[1]}] is not a valid port";
                return;
            }

            var error = TryConnect(port, Slot, ipPortSplit[0], Password);

            if (error is not null)
            {
                State = string.Join("\n", error);
                return;
            }

            State = "";
            File.WriteAllText("ApConnection.txt", $"{Ipporttext}\n{Password}\n{Slot}");
            TimeAccumulator = 0;
        }

        if (IsConnected() && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Disconnect"))
        {
            Disconnect();
        }

        GUI.Label(new Rect(20 + Offset.x, 240 + Offset.y, 300, 30),
            State != "" ? State : IsConnected() ? "Connected" : "Not Connected",
            IsConnected() ? TextStyleGreen : TextStyleRed);
    }

    private void Update() => ApDirtClient.Update();
}