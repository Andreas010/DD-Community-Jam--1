using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DD_JAM.LevelGeneration;

public class TerrainEditor : MonoBehaviour
{
    private Camera cam;
    public StoneRender placeTile;
    public StoneRender airTile;
    public Inventory inventory;
    public GameObject droppedItem;
    public ChunkGeneration chunkGenerator;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!inventory.isInventory && !ShopManager.instance.isOpen && !ConsoleManager.instance.isConsole)
        {
		    if (Input.GetMouseButton(0) && inventory.CanAttack())
                Work(airTile);
            else if (Input.GetMouseButton(1) && inventory.CanPlaceBlock())
                Work(placeTile);
        }
    }

    public void Work(StoneRender tile)
    {
        /*
        
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int curPos = new Vector2Int((int)((mousePosition.x + (mousePosition.x > 0 ? 25f : -25f)) / 50f), (int)((mousePosition.y + (mousePosition.y > 0 ? 25f : -25f)) / 50f));

        chunkGenerator.currentChunks.TryGetValue(curPos, out GameObject chunk);

        if(chunk != null)
        {
            TileMapGenerator generator = chunk.GetComponent<TileMapGenerator>();

            TerrainType curType = generator.GetTile((int)mousePosition.x - curPos.x * 50, (int)mousePosition.y - curPos.y * 50);

            if (curType.item != null && curType.render.internalValue.name != tile.internalValue.name && curType.render.SG != null)
            {
                //Debug.Log($"\"{curType.render.internalValue.name}\"/\"{tile.internalValue.name}\"");

                GameObject item = Instantiate(droppedItem, new Vector2(generator.transform.position.x - 25 + ((int)mousePosition.x - curPos.x * 50) + 0.5f, generator.transform.position.y - 25 + ((int)mousePosition.y - curPos.y * 50) + 0.5f), Quaternion.identity);

                item.GetComponent<SpriteRenderer>().sprite = curType.render.SG.sprite;

                item.GetComponentInChildren<DroppedItem>().inventory = Inventory.instance;
                item.GetComponentInChildren<DroppedItem>().item = curType.item;
                item.GetComponentInChildren<DroppedItem>().count = 1;
            }

            generator.SetTile((int)mousePosition.x - curPos.x*50, (int)mousePosition.y - curPos.y*50, tile);
        }

        */

        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int currentChunk = new Vector2Int((int)((mousePosition.x + (mousePosition.x > 0 ? 25f : -25f)) / 50f), (int)((mousePosition.y + (mousePosition.y > 0 ? 25f : -25f)) / 50f));

        TileMapGenerator curGenerator = chunkGenerator.currentChunks[currentChunk].GetComponent<TileMapGenerator>();

        Vector2Int generatorPos = new Vector2Int((int)mousePosition.x + (25 + (mousePosition.x > 0 ? 0 : -1)) + (currentChunk.x > 0 ? -(Mathf.Abs(currentChunk.x) * 50) : (Mathf.Abs(currentChunk.x) * 50)), (int)mousePosition.y + (25 + (mousePosition.y > 0 ? 0 : -1)) + (currentChunk.y > 0 ? -(Mathf.Abs(currentChunk.y) * 50) : (Mathf.Abs(currentChunk.y) * 50)));

        TerrainType curType = curGenerator.GetTile(generatorPos.x, generatorPos.y);
        curGenerator.SetTile(generatorPos.x, generatorPos.y, tile);

        //(int)mousePosition.x + (25 + (mousePosition.x > 0 ? 0 : -1)) + (currentChunk.x > 0 ? -(Mathf.Abs(currentChunk.x) * 50) : (Mathf.Abs(currentChunk.x) * 50)), (int)mousePosition.y + (25 + (mousePosition.y > 0 ? 0 : -1)) + (currentChunk.y > 0 ? -(Mathf.Abs(currentChunk.y) * 50) : (Mathf.Abs(currentChunk.y) * 50))

        if (curType.item != null && curType.render.internalValue.name != tile.internalValue.name && curType.render.SG != null)
        {
            GameObject item = Instantiate(droppedItem, new Vector2((int)mousePosition.x + (mousePosition.x > 0 ? 0.5f : -0.5f), (int)mousePosition.y + (mousePosition.y > 0 ? 0.5f : -0.5f)), Quaternion.identity, curGenerator.transform);

            item.GetComponent<SpriteRenderer>().sprite = curType.render.SG.sprite;

            item.GetComponentInChildren<DroppedItem>().inventory = Inventory.instance;
            item.GetComponentInChildren<DroppedItem>().item = curType.item;
            item.GetComponentInChildren<DroppedItem>().count = 1;
        }
    }
}
