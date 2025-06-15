#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using PowerwashSimAP.Patches;
using UnityEngine;
using static Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags;

namespace PowerwashSimAP;

public static class ApDirtClient
{
    public static List<long> ChecksToSend = [];
    public static ApClient? Client;
    public static long WinCondition;
    public static int Jobs;
    public static bool HasGoaled;
    public static bool Objectsanity;
    public static bool Percentsanity;
    private static double NextSend = 4;

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        try
        {
            Client = new ApClient();
            Plugin.Log.LogInfo($"Attempting to connect [{address}]:[{port}] [{password}] [{slot}]");

            var connectError = Client.TryConnect(new LoginInfo(port, slot, address, password), 0x3AF4F1BC,
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

        try
        {
            Percentsanity = (bool)slotdata["percentsanity"];
        }
        catch
        {
            Percentsanity = true;
        }

        try
        {
            Objectsanity = (bool)slotdata["objectsanity"];
        }
        catch
        {
            Objectsanity = false;
        }

        JobLevelPatch.Allowed = [Locations.LevelUnlockDictionary[$"{startingLocation} Unlock"]];
        // Plugin.Log.LogInfo($"Raw starting location: [{startingLocation}] win condition: [{WinCondition}]");
        Jobs = 0;

        Plugin.Log.LogInfo("Connnected");
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

        foreach (var item in Client.GetOutstandingItems())
        {
            var locationName = item?.ItemName!;
            // Plugin.Log.LogInfo($"Item gotten: [{locationName}]");

            if (locationName == "A Job Well Done")
            {
                Jobs++;
                return;
            }

            if (!locationName.EndsWith(" Unlock")) return;
            JobLevelPatch.Allowed.Add(Locations.LevelUnlockDictionary[locationName]);
        }

        if (Jobs < WinCondition || HasGoaled) return;
        HasGoaled = true;
        Client.Goal();
    }

    public static bool IsMissing(string locationName)
        => Client is not null && Client.MissingLocations.Any(kv => kv.Value.LocationName.StartsWith(locationName));


    private static void SendChecks()
    {
        Plugin.Log.LogInfo("Send");
        NextSend = 4;
        new Task((Action)(() => TrySendLocations(ChecksToSend))).RunWithTimeout(Client.ServerTimeout);
        ChecksToSend.Clear();
    }

    private static void TrySendLocations(List<long> ids)
    {
        if (Client is null) return;
        if (Client.MissingLocations.Count == 0)
            return;
        Client.Session.Locations.CompleteLocationChecks(ids.Select(id => Client.MissingLocations[id].LocationId)
                                                           .ToArray());
        foreach (var id in ids)
        {
            Client.MissingLocations.Remove(id);
        }
    }
}