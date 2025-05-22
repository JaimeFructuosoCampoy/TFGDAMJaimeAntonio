using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoScript : MonoBehaviour
{
    public SupabaseDAO.InventoryItem ItemInfo;
    private ShopManager Manager;
    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("ShopManager").GetComponent<ShopManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SupabaseDAO.InventoryItem GetItemInfo()
    {
        return ItemInfo;
    }

    public void ShowItemInfo()
    {
        
    }


}
