using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    class InventoryItem
    {
        public Item item;
        public int count;
        public int slotID;

        public InventoryItem(Item item, int count, int slotID)
        {
            this.item = item;
            this.count = count;
            this.slotID = slotID;
        }
    }

    InventoryItem[] items;
    public GameObject[] slots;
    public KeyCode openInventory = KeyCode.E;

    void Start()
    {
        items = new InventoryItem[slots.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new InventoryItem(null, -1, i);
        }
    }

    void Update()
    {
        
    }

    public void AddItem(Item item)
    {
        //TODO: Add Item (Dramatic Music)

        //Find Prefered Slot
        //Check If Item Can Be Added
        //Add Item (Finally)
        //Update HUD
        //Hope It Is Working

        TerrainType terrainType = null;
        Item.ItemType type = item.type;
        int maxCount = item.maxCount;

        switch(type){
            case Item.ItemType.Block:
                terrainType = item.scriptObject as TerrainType;
                break;
            default:
                return;
        }

        int index = 0xDEAD;

        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].item.type != type || items[i].item.type != Item.ItemType.None)
                continue;
            if (items[i].count >= maxCount)
                continue;
            if ((items[i].item.scriptObject as TerrainType).name != terrainType.name)
                continue;
            index = i;
        }

        if(index == 0xDEAD) //Only the case if the item doesn't fit in your inventory
        {
            return;

            //TODO: Ask user if he wants to replace the item with another item in inventory
        }
    }
}
