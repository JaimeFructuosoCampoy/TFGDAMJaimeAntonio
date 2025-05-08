using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SupabaseDAO;

public class PlayerLoggedIn : MonoBehaviour
{
    public static string PlayerId { get; set; }
    public static string PlayerName { get; set; }
    public static DateTime CreatedAt { get; set; }
    public static int Coins { get; set; }
    public static int Points { get; set; }
    public static int Diamonds { get; set; }
    public static List<InventoryItem> Inventory { get; set; }

    public static void InitializeOrUpdatePlayerData(PlayerData data)
    {
        PlayerId = data.id;
        PlayerName = data.player_friendly_name;
        CreatedAt = data.created_at;
        Coins = data.coins;
        Points = data.points;
        Diamonds = data.diamonds;
        Inventory = data.Inventory ?? new List<InventoryItem>();
    }

    public static void ClearPlayerData()
    {
        PlayerId = null;
        PlayerName = null;
        CreatedAt = DateTime.MinValue;
        Coins = 0;
        Points = 0;
        Diamonds = 0;
        Inventory = new List<InventoryItem>();
    }

}
