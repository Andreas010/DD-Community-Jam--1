using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Inventory : MonoBehaviour
{
    public class InventoryItem
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
    public GameObject stats;
    public GameObject drop;
    public GameObject droppedItem;

    public Item test1;
    public Item test2;
    public Item testWeapon;

    InventoryItem selected;
    InventoryItem selectedWeapon;
    InventoryItem currItem;
    InventoryItem currStatItem;

    TerrainEditor editor;
    PlayerMovement movement;

    Item defaultItem;

    Slider slider;
    TMP_InputField altInput;

    public bool isInventory = false;
    bool isStatsActive = false;
    bool first = true;
    bool firstWeapon = true;

    bool slotIsLeft;
    bool slotIsTop;

    float defRange;

    int itemsToDrop;

    public Transform player;

    void Start()
    {
        items = new InventoryItem[slots.Length];
        editor = FindObjectOfType<TerrainEditor>();
        movement = FindObjectOfType<PlayerMovement>();
        selected = null;
        defRange = editor.range;

        transform.GetChild(0).gameObject.SetActive(false);
        info.SetActive(false);
        stats.SetActive(false);
        drop.SetActive(false);

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
            {
                info.SetActive(false);
                stats.SetActive(false);
                drop.SetActive(false);
            }
        }
    }

    public bool CanPlaceBlock() => selected != null;

    void InventoryGUI()
    {
        info.SetActive(false);
        stats.SetActive(false);

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
        {
            info.SetActive(false);
            return;
        }

        if (isStatsActive && currStatItem != item)
        {
            stats.SetActive(false);
            isStatsActive = false;
        }

        currItem = item;

        ComputeSlotPosition(index);

        int offX = 0;
        int offY = 0;

        if (slotIsLeft && !slotIsTop) { offX = 90; offY = 90; }
        else if (!slotIsLeft && !slotIsTop) { offX = -90; offY = 90; }
        else if (slotIsLeft && slotIsTop) { offX = 90; offY = -90; }
        else if (!slotIsLeft && slotIsTop) { offX = -90; offY = -90; }

        info.transform.position = new Vector3(slots[index].transform.position.x + offX, slots[index].transform.position.y + offY, 0);
        info.SetActive(true);

        info.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = item.item.displayName;

        info.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = $"Count: {item.count}";

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

        if(item.item.type == Item.ItemType.Block)
        {
            info.transform.GetChild(1).GetComponent<Button>().interactable = false;

            if(!hasValidID)
                info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Select As Building Block";
        }

        else if(item.item.type == Item.ItemType.Weapon)
        {
            info.transform.GetChild(1).GetComponent<Button>().interactable = true;

            if(isStatsActive)
                info.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Hide Stats";
            else
                info.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Show Stats";

            if(!hasValidID)
                info.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Select As Weapon";
        }
    }

    void ComputeSlotPosition(int slotID)
    {
        switch (slotID)
        {
            case 0:
            case 1:
            case 2:
            case 5:
            case 6:
            case 7:
            case 10:
            case 11:
            case 12:
            case 15:
            case 16:
            case 17:
                slotIsLeft = true;
                break;
            default:
                slotIsLeft = false;
                break;
        }

        if (slotID > 9)
            slotIsTop = true;
        else
            slotIsTop = false;
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

    public void ShowStats()
    {
        isStatsActive = !isStatsActive;

        if (!isStatsActive)
        {
            stats.SetActive(false);
            Clicked(currItem.slotID);
            return;
        }

        InventoryItem item = currItem;

        currStatItem = item;

        if (item.item.type == Item.ItemType.None || item.item.type == Item.ItemType.Block)
            return;

        Clicked(currStatItem.slotID);

        stats.SetActive(true);

        info.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Hide Stats";

        Transform statsText = stats.transform.GetChild(0);

        statsText.gameObject.GetComponent<TMP_Text>().text = $"Cooldown: {(currStatItem.item.scriptObject as Weapon).weaponCooldown}\n";
        statsText.gameObject.GetComponent<TMP_Text>().text += $"Speed: {(currStatItem.item.scriptObject as Weapon).weaponSpeed}\n";
        statsText.gameObject.GetComponent<TMP_Text>().text += $"Damage: {(currStatItem.item.scriptObject as Weapon).weaponDamage}\n";
        statsText.gameObject.GetComponent<TMP_Text>().text += $"Knockback: {(currStatItem.item.scriptObject as Weapon).weaponKnockback}\n";
        statsText.gameObject.GetComponent<TMP_Text>().text += $"Mine Speed: {(currStatItem.item.scriptObject as Weapon).mineSpeed}";
    }

    bool IsMiddleBar(int slotID)
    {
        switch (slotID)
        {
            case 2:
            case 7:
            case 12:
            case 17:
                return true;
            default:
                return false;
        }
    }

    public void RemoveItem(int slotID, int count)
    {
        InventoryItem item = items[slotID];

        if (item.item.type == Item.ItemType.None || currItem == null)
            return;

        if (item.count - count <= 0)
        {
            DeleteItem(slotID);
        }
        else
            item.count -= count;

        if (currItem.slotID == slotID)
            Clicked(slotID);
    }

    public InventoryItem GetFirstItem(Item.ItemType type)
    {
        foreach(InventoryItem i in items)
        {
            if (i.item.type == type)
                return i;
        }

        return null;
    }

    public void DeleteItem(int slotID)
    {
        InventoryItem item = null;

        if (slotID == -1)
            item = currItem;
        else
            item = items[slotID];

        item.item = defaultItem;
        item.count = 0;

        if (selected != null)
        {
            if (item == selected)
            {
                InventoryItem altItem = GetFirstItem(Item.ItemType.Block);

                if (altItem != null)
                    selected = altItem;
                else
                    selected = null;
            }
        }

        if (selectedWeapon != null)
        {
            if (item == selectedWeapon)
            {
                InventoryItem altItem = GetFirstItem(Item.ItemType.Weapon);

                if (altItem != null)
                    selectedWeapon = altItem;
                else
                    selectedWeapon = null;
            }
        }


        if (isInventory)
        {
            info.SetActive(false);
            InventoryGUI();
        }
    }

    public bool CanAttack() => selectedWeapon != null;

    public void Drop()
    {
        InventoryItem item = currItem;

        if (item == null)
            return;

        if(item.item.type == Item.ItemType.Weapon || item.count == 1)
        {
            itemsToDrop = 1;
            EndDrop();
            return;
        }

        drop.SetActive(true);

        slider = drop.transform.GetChild(2).gameObject.GetComponent<Slider>();
        altInput = drop.transform.GetChild(3).gameObject.GetComponent<TMP_InputField>();

        slider.maxValue = item.count;
        altInput.text = "0";
    }

    public void EndDrop()
    {
        drop.SetActive(false);

        GameObject item = Instantiate(droppedItem, player.position, Quaternion.identity);
        if(currItem.item.type == Item.ItemType.Block)
            item.GetComponent<SpriteRenderer>().sprite = (currItem.item.scriptObject as TerrainType).tile.sprite;
        else if (currItem.item.type == Item.ItemType.Weapon)
            item.GetComponent<SpriteRenderer>().sprite = (currItem.item.scriptObject as Weapon).sprite;

        item.GetComponentInChildren<DroppedItem>().inventory = this;
        item.GetComponentInChildren<DroppedItem>().item = currItem.item;
        item.GetComponentInChildren<DroppedItem>().count = itemsToDrop;

        item.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<SpriteRenderer>().flipX ? -5 : 5, 3);

        RemoveItem(currItem.slotID, itemsToDrop);
    }

    public void UpdateDropCounter(bool isSlider)
    {
        if (isSlider)
        {
            itemsToDrop = (int)slider.value;
            altInput.text = itemsToDrop.ToString();
        }

        else if(altInput.text != itemsToDrop.ToString())
        {

            if (int.TryParse(altInput.text, out int parser))
            {
                slider.value = parser;
                itemsToDrop = parser;
            }

            else
                altInput.text = itemsToDrop.ToString();
        }
    }
}