#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using UnityEngine;
using static Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags;
using static CreepyUtil.Archipelago.ApClient.ApClient;
using static PowerwashSimAP.Locations;

namespace PowerwashSimAP;

public static class ApDirtClient
{
    private static List<string> ChecksToSend = [];
    public static ConcurrentQueue<string> ChecksToSendQueue = [];
    public static ApClient? Client;
    public static long WinCondition;
    public static int Jobs;
    public static bool Objectsanity;
    public static bool Percentsanity;
    public static List<string> Allowed = [];
    public static string[] GoalLevelsOpen = [];
    public static GoalType Goal = 0;
    public static long LevelCount = -1;
    public static string[] Levels = [];
    public static long CompletedLevelCount;
    public static HashSet<string> CachedLevelsCompleted = [];
    private static double NextSend = 4;
    private static Dictionary<string, Regex> RegexCache = [];
    
    public enum GoalType
    {
        McGuffinHunt = 0,
        LevelHunt = 1
    }

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        try
        {
            Client = new ApClient();
            Plugin.Log.LogInfo($"Attempting to connect [{address}]:[{port}] [{password}] [{slot}]");

            var connectError = Client.TryConnect(new LoginInfo(port, slot, address, password),
                "Powerwash Simulator", AllItems, requestSlotData: true);

            if (connectError is not null && connectError.Length > 0)
            {
                Plugin.Log.LogInfo("There was an Error");
                Disconnect();
                return connectError;
            }

            HasConnected();
        }
        catch (Exception e)
        {
            Plugin.Log.LogInfo("There was an Error");
            Disconnect();
            return [e.Message, e.StackTrace!];
        }

        return null;
    }

    public static void Disconnect()
    {
        Client?.TryDisconnect();
        Client = null;
        Plugin.Log.LogInfo("Disconnected");
    }

    public static void HasConnected()
    {
        var slotdata = Client?.SlotData!;
        var startingLocation = (string)slotdata["starting_location"]!;
        WinCondition = (long)slotdata["jobs_done"];
        if (slotdata.TryGetValue("percentsanity", out var temp)) Percentsanity = (bool)temp;
        if (slotdata.TryGetValue("objectsanity", out var temp1)) Objectsanity = (bool)temp1;

        if (slotdata.TryGetValue("goal_levels", out var temp3))
        {
            Levels = ((string)temp3).Split(',').Select(s => s.Trim('\'', '[', ']', ' ', '"')).ToArray();
        }

        if (slotdata.TryGetValue("goal_level_amount", out var temp4)) LevelCount = (long)temp4;
        if (LevelCount == 0) LevelCount = Levels.Length;

        Goal = Levels.Any() && Levels[0] != "None" ? GoalType.LevelHunt : GoalType.McGuffinHunt;
        // Plugin.Log.LogInfo($"[{Goal}] | [{string.Join(", ", Levels)}] | [{startingLocation}]");

        Allowed = [LevelDictionary[startingLocation]];
        Plugin.Log.LogInfo($"Starting: [{LabelNameToLocationName[Allowed[0]]}]");
        Jobs = 0;


        Plugin.Log.LogInfo("Receiving Items");
        ReceiveItems();
        Plugin.Log.LogInfo("Connected, running failsafe checks");

        CachedLevelsCompleted = Client!.GetFromStorage<string[]>("levels_completed", def: [])!.ToHashSet();
        Plugin.Log.LogInfo($"Levels Completed: [{CachedLevelsCompleted.Count}]:[{string.Join(", ", CachedLevelsCompleted)}]");
        GoalLevelCheck(CachedLevelsCompleted.ToArray());
        
        Plugin.Log.LogInfo($"Completed Levels ({CachedLevelsCompleted.Count}): \n> {string.Join("\n> ", CachedLevelsCompleted.Select(s => $"[{s}]"))}");
        foreach (var unlock in CachedLevelsCompleted)
        {
            FailsafeSendLocations(unlock);
        }

        Plugin.Log.LogInfo("Failsafe checking complete");
    }

    public static bool IsConnected()
    {
        return Client is not null && Client.IsConnected;
    }

    public static void Update()
    {
        if (Client is null) return;
        Client.UpdateConnection();
        if (!Client.IsConnected) return;

        NextSend -= DeltaTime;
        if (ChecksToSend.Any() && NextSend <= 0)
        {
            SendChecks();
        }

        ReceiveItems();

        while (!ChecksToSendQueue.IsEmpty)
        {
            ChecksToSendQueue.TryDequeue(out var location);
            ChecksToSend.Add(location);
        }

        if (Jobs < WinCondition) return;
        Client.Goal();
    }

    private static void ReceiveItems()
    {
        var rawNewItems = Client!.GetOutstandingItems().ToArray();
        if (!rawNewItems.Any()) return;
        var newItems = rawNewItems
                      .Where(item => item?.Flags != 0)
                      .Select(item => item?.ItemName!)
                      .ToArray();

        Jobs += newItems.Count(item => item == "A Job Well Done");
        var newAllowed = newItems.Where(item => item.EndsWith(" Unlock"))
                                 .Select(item => LevelUnlockDictionary[item])
                                 .ToArray();

        if (!newAllowed.Any()) return;
        Allowed.AddRange(newAllowed);
        Plugin.Log.LogInfo($"New allowed: [{string.Join(", ", newAllowed.Select(s => LabelNameToLocationName[s]))}]");
        UpdateAvailableLevelGoal();
    }

    public static bool IsMissingNonStrict(string locationName)
    {
        var regex = LevelRegex(locationName);
        return Client is not null && Client.MissingLocations.Any(id => regex.IsMatch(id));
    }

    public static bool IsMissing(string locationName)
        => Client is not null && Client.MissingLocations.Contains(locationName);

    private static void SendChecks()
    {
        NextSend = 3;
        Plugin.Log.LogInfo($"Send checks: [{string.Join(", ", ChecksToSend)}]");
        Client?.SendLocations(ChecksToSend.ToArray());
        ChecksToSend.Clear();
    }

    public static void SetLevelCompletion(string level)
    {
        try
        {
            if (Client is null) return;
            Plugin.Log.LogInfo("Set Level Completion");
            CachedLevelsCompleted.Add(level);
            Client.SendToStorage("levels_completed", CachedLevelsCompleted.ToArray());
            if (Goal is GoalType.McGuffinHunt) return;
            GoalLevelCheck(CachedLevelsCompleted.ToArray());
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    public static void GoalLevelCheck(string[] levelsCompleted)
    {
        if (Client is null) return;
        Plugin.Log.LogInfo($"Goal Level Check: [{Goal}]");
        if (Goal is GoalType.McGuffinHunt) return;
        UpdateAvailableLevelGoal();
        if (LevelCount > levelsCompleted.Count(s => Levels.Contains(s))) return;
        Client.Goal();
    }

    public static void UpdateAvailableLevelGoal()
    {
        if (Client is null) return;
        if (Goal is GoalType.McGuffinHunt) return;
        var separatedLevels = Levels.GroupBy(str => CachedLevelsCompleted.Contains(str)).ToArray();
        try
        {
            CompletedLevelCount = separatedLevels.FirstOrDefault(g => g.Key)?.Count() ?? 0;
            GoalLevelsOpen = separatedLevels.FirstOrDefault(g => !g.Key)?.ToArray() ?? [];
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
        Plugin.Log.LogInfo("Update Avail Level Goal");
        Plugin.Log.LogInfo($"Sep: [{separatedLevels.Length}]: [{string.Join(", ", separatedLevels.Select(g => $"({g.Key} ,{g.Count()})"))}]");
        Plugin.Log.LogInfo($"Complete: [{CompletedLevelCount}]");
        Plugin.Log.LogInfo($"Open: [{GoalLevelsOpen.Length}]: [{string.Join(", ", GoalLevelsOpen)}]");
    }

    public static void FailsafeSendLocations(string locationName)
    {
        Plugin.Log.LogInfo($"Level Finished Detected: [{locationName}] running failsafe");
        var reg = LevelRegex(locationName);

        var missing = 0;
        foreach (var loc in Client!.MissingLocations.Where(id=> reg.IsMatch(id)))
        {
            if (ChecksToSendQueue.Contains(loc)) continue;
            ChecksToSendQueue.Enqueue(loc);
            missing++;
        }

        Plugin.Log.LogInfo($"Failsafe Finished, Sent [{missing}] failed checks");
    }

    public static Regex LevelRegex(string locationName)
    {
        if (RegexCache.TryGetValue(locationName, out var regex)) return regex;
        return RegexCache[locationName] = new Regex(@$"^{locationName.Replace("(", "\\(").Replace(")", "\\)")}( \d{{1,3}}%$|: )", RegexOptions.Compiled);
    }
}