using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DD_JAM.LevelGeneration;

public class TerrainEditor : MonoBehaviour
{
    public float range;
    private Camera cam;
    public TerrainType placeTile;
    public TerrainType airTile;
    public Inventory inventory;

    public LayerMask chunkLayer;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!inventory.isInventory)
        {
		    if (Input.GetMouseButton(0) && inventory.CanAttack())
                Work(airTile);
            else if (Input.GetMouseButton(1) && inventory.CanPlaceBlock())
                Work(placeTile);
        }
    }

    public void Work(TerrainType tile)
    {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] chunks = Physics2D.OverlapCircleAll(mousePosition, range, chunkLayer);
        List<TileMapGenerator> generators = new List<TileMapGenerator>();

        for (int i = 0; i < chunks.Length; i++)
        {
            TileMapGenerator tmpGen = chunks[i].GetComponent<TileMapGenerator>();
            if (tmpGen != null)
                generators.Add(tmpGen);
        }

        for (int i = 0; i < generators.Count; i++)
        {
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    float distance = Vector2.Distance(new Vector2(generators[i].transform.position.x - 25 + x + 0.5f, generators[i].transform.position.y - 25 + y + 0.5f), mousePosition);

                    if (distance < range / 2) {
						//GetComponent<Inventory>().AddItem(generators[i].GetTile(x, y));
						generators[i].SetTile(x, y, tile);
					}
                }
            }
        }
    }
}
