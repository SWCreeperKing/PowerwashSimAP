using UnityEngine;

namespace PowerwashSimAP.Patches;

public static class Helper
{
    public static GameObject[] GetChildren(this GameObject gobj)
    {
        var transform = gobj.transform;
        var count = transform.childCount;
        var children = new GameObject[count];

        for (var i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
        
        return children;
    }
}