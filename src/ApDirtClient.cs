#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CreepyUtil.Archipelago;
using UnityEngine;
using static Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags;

namespace PowerwashSimAP;

public static class ApDirtClient
{
    public static ApClient? Client;

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        try
        {
            Client = new ApClient();
            Plugin.Log.LogInfo($"Attempting to connect [{address}]:[{port}] [{password}] [{slot}]");

            var connectError = Client.TryConnect(new LoginInfo(port, slot, address, password), 0x0BF6E0BB,
                "Powerwash Simulator", AllItems);

            if (connectError is not null && connectError.Length > 0)
            {
                Plugin.Log.LogInfo($"There was an Error");
                Disconnect();
                return connectError;
            }

            Reset();
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

    public static void HasConnected() { Plugin.Log.LogInfo("Connnected"); }

    public static bool IsConnected()
    {
        return Client is not null && Client.IsConnected && Client.Session.Socket.Connected;
    }

    public static void Reset() { }

    // public static void Update(GeneralManager? manager)
    // {
    //     var delta = Time.deltaTime;
    //
    //     if (Client is null) return;
    //     Client.UpdateConnection();
    //     if (Client?.Session?.Socket is null || !Client.IsConnected) return;
    //
    //     foreach (var item in Client.GetOutstandingItems())
    //     {
    //         switch (item!.ItemName)
    //         {
    //             case "Progressive Column Unlock":
    //                 ColumnCount++;
    //                 UpdateColumns();
    //                 break;
    //             case "Progressive Spawn Speed Upgrade":
    //                 SpeedCount++;
    //                 break;
    //             case "Random Duck":
    //                 DuckCount++;
    //                 break;
    //         }
    //     }
    //
    //     if (manager is null) return;
    //     if (!HasStartingDuck)
    //     {
    //         HasStartingDuck = true;
    //         manager.basicSpawner.SpawnDuck("Duck1Base", false, "", -2);
    //     }
    //
    //     Timer += delta;
    //     if (Timer >= 120 - SpeedCount * 10)
    //     {
    //         Timer = 0;
    //         manager.basicSpawner.SpawnDuck(GetNeededDuck(), false, "", -2);
    //     }
    //
    //     if (DuckCount <= 0) return;
    //     if (manager.basicSpawner is null) return;
    //     manager.basicSpawner.SpawnDuck("Random Duck", false, playerID: -2);
    //     DuckCount--;
    // }
}