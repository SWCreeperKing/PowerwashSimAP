#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CreepyUtil.Archipelago;
using UnityEngine;
using static Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags;
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
    private static double NextSend = 4;

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

        Allowed = [LevelUnlockDictionary[$"{startingLocation} Unlock"]];
        Jobs = 0;

        GoalLevelCheck(Client!.GetFromStorage<string[]>("levels_completed", def: [])!);

        Plugin.Log.LogInfo("Receiving Items");
        ReceiveItems();
        Plugin.Log.LogInfo("Connected, running failsafe checks");

        foreach (var unlock in Allowed.Select(unlock => LabelNameToLocationName[unlock])
                                      .Where(unlock => !IsMissing($"{unlock} 100%")))
        {
            FailsafeSendLocations(unlock);
        }

        Plugin.Log.LogInfo("Failsafe checking complete");
    }

    public static bool IsConnected()
    {
        return Client is not null && Client.IsConnected && Client.Session.Socket.Connected;
    }

    public static void Update()
    {
        if (Client is null) return;
        Client.UpdateConnection();
        if (Client?.Session?.Socket is null || !Client.IsConnected) return;

        NextSend -= Time.deltaTime;
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
        Plugin.Log.LogInfo($"Unlocked: [{string.Join(", ", newAllowed)}]");
        UpdateAvailableLevelGoal();
    }

    public static bool IsMissingStartsWith(string locationName)
        => Client is not null && Client.MissingLocations.Any(id => Client.Locations[id].StartsWith(locationName));

    public static bool IsMissing(string locationName)
        => Client is not null && Client.MissingLocations.Contains(Client.Locations[locationName]);

    private static void SendChecks()
    {
        NextSend = 3;
        Client?.SendLocations(ChecksToSend.ToArray());
        ChecksToSend.Clear();
    }

    public static void SetLevelCompletion(string level)
    {
        if (Client is null) return;
        if (Goal is GoalType.McGuffinHunt) return;
        var data = Client.GetFromStorage<string[]>("levels_completed", def: [])!;
        if (data.Contains(level))
        {
            GoalLevelCheck(data);
            return;
        }

        Client.SendToStorage("levels_completed", data.Append(level).ToArray());
        GoalLevelCheck(data);
    }

    public static void GoalLevelCheck(string[] levelsCompleted)
    {
        if (Client is null) return;
        if (Goal is GoalType.McGuffinHunt) return;
        UpdateAvailableLevelGoal();
        if (LevelCount > levelsCompleted.Count(s => Levels.Contains(s))) return;
        Client.Goal();
    }

    public static void UpdateAvailableLevelGoal()
    {
        if (Client is null) return;
        if (Goal is GoalType.McGuffinHunt) return;
        var data = Client.GetFromStorage<string[]>("levels_completed", def: [])!;
        var seperatedLevels = Levels.GroupBy(str => data.Contains(str)).ToArray();
        try
        {
            CompletedLevelCount = seperatedLevels.FirstOrDefault(g => g.Key)?.Count() ?? 0;
            GoalLevelsOpen = seperatedLevels.FirstOrDefault(g => !g.Key)?.ToArray() ?? [];
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    public static void FailsafeSendLocations(string locationName)
    {
        Plugin.Log.LogInfo($"Level Finished Detected: [{locationName}] running failsafe");
        var reg = new Regex(@$"^{locationName}( \d{{1,3}}%$|: )", RegexOptions.Compiled);

        var missing = 0;
        foreach (var loc in Client!.MissingLocations.Where(id
                                        => reg.IsMatch(Client.Locations[id]))
                                   .Select(loc => Client.Locations[loc]))
        {
            if (ChecksToSendQueue.Contains(loc)) continue;
            ChecksToSendQueue.Enqueue(loc);
            missing++;
        }
        Plugin.Log.LogInfo($"Failsafe Finished, Sent [{missing}] failed checks");
    }
}