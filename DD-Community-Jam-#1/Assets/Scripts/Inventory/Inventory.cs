using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public Image[] slotImg;
    [HideInInspector]
    public Color defColor;
    public GameObject info;
    public Item test1;
    public Item test2;

    bool isInventory = false;

    void Start()
    {
        items = new InventoryItem[slots.Length];

        transform.GetChild(0).gameObject.SetActive(false);
        info.SetActive(false);

        defColor = slotImg[0].color;

        Item defaultItem = ScriptableObject.CreateInstance("Item") as Item;
        defaultItem.maxCount = 0;
        defaultItem.type = Item.ItemType.None;
        defaultItem.scriptObject = null;
        defaultItem.displayName = "None";

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new InventoryItem(defaultItem, 0, i);
        }

        AddItem(test1, 6);
        AddItem(test2, 2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
            isInventory = !isInventory;
            if(isInventory)
                InventoryGUI();
            else
                info.SetActive(false);
        }

        
    }

    void InventoryGUI()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].item.type == Item.ItemType.Block)
            {
                slotImg[i].sprite = (items[i].item.scriptObject as TerrainType).tile.sprite;
                slotImg[i].color = new Color(1, 1, 1, 1);
            }

            else if(items[i].item.type == Item.ItemType.None)
            {
                slotImg[i].color = defColor;
            }
        }
    }

    public void AddItem(Item item, int count)
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
        int voidIndex = 0xDEAD;

        for(int i = 0; i < items.Length; i++)
        {
            if(items[i].item.type == Item.ItemType.None)
            {
                voidIndex = i;
                continue;
            }
            if (items[i].item.type != type)
                continue;
            if (items[i].count + count > maxCount)
                continue;
            if ((items[i].item.scriptObject as TerrainType).name != terrainType.name)
                continue;
            index = i;
        }

        if (index == 0xDEAD && voidIndex == 0xDEAD) //Only the case if the item doesn't fit in your inventory
        {
            return;

            //TODO: Ask user if he wants to replace the item with another item in inventory
        }

        else if (index == 0xDEAD)
            index = voidIndex;

        items[index] = new InventoryItem(item, count + items[index].count, index);
    }

    public void Clicked(int index)
    {
        InventoryItem item = items[index];

        if (item.item.type == Item.ItemType.None)
            return;

        info.transform.position = new Vector3(slots[index].transform.position.x - 90, slots[index].transform.position.y - 90, 0);
        info.SetActive(true);

        info.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = item.item.displayName;

        info.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = $"Count: {item.count}";
    }
}
