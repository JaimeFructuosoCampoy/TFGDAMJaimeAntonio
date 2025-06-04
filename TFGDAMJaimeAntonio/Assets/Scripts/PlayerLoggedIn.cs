using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SupabaseDao;

public class PlayerLoggedIn : MonoBehaviour
{
    public static string PlayerId { get; set; }
    public static string FriendlyName { get; set; }
    public static string CreatedAt { get; set; }
    public static int Coins { get; set; }
    public static int Points { get; set; }
    public static int Diamonds { get; set; }
    public static List<InventoryItem> Inventory { get; set; }

    public static void InitializeOrUpdatePlayerData(PlayerData data)
    {
        PlayerId = data.id;
        FriendlyName = data.friendly_name;
        CreatedAt = data.created_at;
        Coins = data.coins;
        Points = data.points;
        Diamonds = data.diamonds;
    }

    public static void InitializeOrUpdateInventory(List<InventoryItem> inventory)
    {
        Inventory = inventory;
    }

    public static void ClearPlayerData()
    {
        PlayerId = null;
        FriendlyName = null;
        CreatedAt = null;
        Coins = 0;
        Points = 0;
        Diamonds = 0;
        Inventory = new List<InventoryItem>();
    }
    
}
