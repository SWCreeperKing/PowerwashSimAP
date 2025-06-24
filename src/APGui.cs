using System.IO;
using System.Linq;
using PowerwashSimAP.Patches;
using UnityEngine;
using static PowerwashSimAP.ApDirtClient;

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
    public static bool WasPressed;

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
        if (Plugin.IsDebug is Plugin.DebugWant.Washables) return;
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
                GUI.Label(new Rect(20 + Offset.x, Offset.y + 155, 150, 35), $"Level of interest:\n[{GoalLevelsOpen[0]}]", TextStyle);
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