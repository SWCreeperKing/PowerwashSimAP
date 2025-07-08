using System.Collections.Generic;
using System.Linq;

namespace PowerwashSimAP;

public static class Locations
{
    public static string[][] RawLocationData = [];
    public static Dictionary<string, string[]> CleanParts;
    public static Dictionary<string, string> LevelUnlockDictionary;
    public static Dictionary<string, string> LevelDictionary;
    public static Dictionary<string, string> SceneNameToLocationName;
    public static Dictionary<string, string> LabelNameToLocationName;
}