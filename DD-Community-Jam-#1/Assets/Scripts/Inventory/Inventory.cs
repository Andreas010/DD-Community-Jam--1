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
    Color defColor;
    public GameObject info;
    public Item test1;
    public Item test2;
    public Item testWeapon;

    InventoryItem selected;
    InventoryItem selectedWeapon;
    InventoryItem currItem;

    TerrainEditor editor;
    PlayerMovement movement;

    Item defaultItem;

    bool isInventory = false;
    bool first = true;
    bool firstWeapon = true;

    float defRange;

    void Start()
    {
        items = new InventoryItem[slots.Length];
        editor = FindObjectOfType<TerrainEditor>();
        movement = FindObjectOfType<PlayerMovement>();
        selected = null;
        defRange = editor.range;

        transform.GetChild(0).gameObject.SetActive(false);
        info.SetActive(false);

        defColor = slotImg[0].color;

        defaultItem = ScriptableObject.CreateInstance("Item") as Item;
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
        AddItem(testWeapon, 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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
            if(items[i].item.type == Item.ItemType.None)
            {
                slotImg[i].color = defColor;
                slotImg[i].sprite = null;
            }

            else if (items[i].item.type == Item.ItemType.Block)
            {
                slotImg[i].sprite = (items[i].item.scriptObject as TerrainType).tile.sprite;
                slotImg[i].color = new Color(1, 1, 1, 1);
            }

            else if(items[i].item.type == Item.ItemType.Weapon)
            {
                slotImg[i].sprite = (items[i].item.scriptObject as Weapon).sprite;
                slotImg[i].color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void AddItem(Item item, int count)
    {
        TerrainType terrainType = null;
        Item.ItemType type = item.type;
        int maxCount = item.maxCount;

        switch(type){
            case Item.ItemType.Block:
                terrainType = item.scriptObject as TerrainType;
                break;
            default:
                break;
        }

        //Spagetti-Code

        int index = 0xDEAD;
        int voidIndex = 0xDEAD;

        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].item.type == Item.ItemType.None)
            {
                voidIndex = i;
                continue;
            }
            if (type == Item.ItemType.Weapon)
                continue;
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

        if (first && type == Item.ItemType.Block)
        {
            SelectAsBlock(items[index]);
            first = false;
        }

        else if(firstWeapon && type == Item.ItemType.Weapon)
        {
            SelectAsWeapon(items[index]);
            firstWeapon = false;
        }

        if (isInventory)
        {
            info.SetActive(false);
            InventoryGUI();
        }
    }

    public void Clicked(int index)
    {
        InventoryItem item = items[index];

        if (item.item.type == Item.ItemType.None)
            return;

        currItem = item;

        info.transform.position = new Vector3(slots[index].transform.position.x - 90, slots[index].transform.position.y - 90, 0);
        info.SetActive(true);

        info.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = item.item.displayName;

        info.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = $"Count: {item.count}";

        info.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = true;

        bool hasValidID = false;

        if(selected != null)
        {
            if (selected.slotID == item.slotID)
            {
                info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Already Selected";
                info.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
                hasValidID = true;
            }
        }

        if(selectedWeapon != null)
        {
            if (selectedWeapon.slotID == item.slotID)
            {
                info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Already Selected";
                info.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
                hasValidID = true;
            }
        }

        if(item.item.type == Item.ItemType.Block && !hasValidID)
        {
            info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Select As Building Block";
        }

        else if(item.item.type == Item.ItemType.Weapon)
        {
            info.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Show Stats";
            if(!hasValidID)
                info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Select As Weapon";
        }
    }

    void SelectAsBlock(InventoryItem item)
    {
        editor.placeTile = item.item.scriptObject as TerrainType;
        selected = item;
        if (currItem == item)
            Clicked(item.slotID);
    }

    void SelectAsWeapon(InventoryItem item)
    {
        movement.weapon = item.item.scriptObject as Weapon;
        selectedWeapon = item;
        if (currItem == item)
            Clicked(item.slotID);
    }

    public void ClickedToSelect()
    {
        editor.range = defRange;

        switch (currItem.item.type)
        {
            case Item.ItemType.Block:
                SelectAsBlock(currItem);
                break;
            case Item.ItemType.Weapon:
                SelectAsWeapon(currItem);
                break;
            default:
                break;
        }
    }

    public void RemoveItem(int slotID, int count)
    {
        InventoryItem item = items[slotID];

        if (item.item.type == Item.ItemType.None || currItem == null)
            return;

        if (item.count - count <= 0)
        {
            if(slotID == selected.slotID)
            {
                selected = null;
                editor.range = 0;
            }

            else if(slotID == selectedWeapon.slotID)
            {
                selectedWeapon = null;
                //TODO: Deactivate Weapon usage
            }

            item.item = defaultItem;
            item.count = 0;

            if (isInventory)
            {
                info.SetActive(false);
                InventoryGUI();
            }
        }
        else
            item.count -= count;

        if (currItem.slotID == slotID)
            Clicked(slotID);
    }
}