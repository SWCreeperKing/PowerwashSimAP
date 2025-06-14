using UnityEngine;

namespace PowerwashSimAP;

public class ApDisconnectWarn : MonoBehaviour
{
    public static GUIStyle TextStyle = new()
    {
        fontSize = 32,
        normal =
        {
            textColor = Color.red
        }
    };

    private void OnGUI()
    {
        if (ApDirtClient.IsConnected()) return;
        GUI.Box(new Rect(140, 140, 610, 105), "Ap Disconnection Warn Box");
        GUI.Label(new Rect(145, 165, 240, 30), "DISCONNECTED FROM ARCHIPELAGO", TextStyle);
        GUI.Label(new Rect(145, 205, 240, 30), "PLEASE SAVE AND RELOAD SAVE", TextStyle);
    }
}