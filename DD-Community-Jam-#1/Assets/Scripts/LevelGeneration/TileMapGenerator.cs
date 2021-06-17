using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DD_JAM.LevelGeneration
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    [RequireComponent(typeof(Grid))]
    public class TileMapGenerator : MonoBehaviour
    {
        [Header("What color takes which spot?")]
        public Gradient colorLevel;
        [Header("What does exist?")]
        public TerrainType[] types;

        private Tilemap tilemap;
        private Vector2Int levelSize;
        private TerrainType[,] curLevel;
        private float curThreshhold;

        public void ReGenerate(TerrainType[,] level)
        {
            curLevel = level;
            Generate();
        }

        public void Build(float[,] level, float threshhold)
        {
            if (types.Length == 0)
                return;

            levelSize = new Vector2Int(level.GetLength(0), level.GetLength(1));

            curLevel = new TerrainType[levelSize.x, levelSize.y];

            for (int x = 0; x < levelSize.x; x++)
            {
                for (int y = 0; y < levelSize.y; y++)
                {
                    if (types.Length == 1)
                    {
                        //tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, x - (levelSize.x / 2) + transform.position.y)), types[0].tile);
                        curLevel[x, y] = types[0];
                        continue;
                    }

                    float value = level[x, y];

                    value = QuickMaths(value, threshhold);

                    Color tmpColor = colorLevel.Evaluate(value);
                    int index = 0xDEAD;

                    for (int i = 0; i < types.Length; i++)
                    {
                        if (IsColor(types[i].value, tmpColor))
                            index = i;
                    }

                    if (index != 0xDEAD)
                    {
                        //tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), types[index].tile);
                        curLevel[x, y] = types[index];
                    }
                }
            }
        }

        public void Generate()
        {
            tilemap = GetComponent<Tilemap>();

            for (int x = 0; x < levelSize.x; x++)
            {
                for (int y = 0; y < levelSize.y; y++)
                {
                    tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), curLevel[x, y].tile);
                }
            }
        }

        public void SetTile(int x, int y, TerrainType tile)
        {
            Tile tmpTile = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (IsColor(types[i].value, tile.value))
                {
                    tmpTile = types[i].tile;
                }
            }

            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), tmpTile);
            curLevel[x, y] = tile;
        }
        public TerrainType[,] GetLevel()
        {
            return curLevel;
        }
        public Tile GetTile(int x, int y)
        {
            return tilemap.GetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y))) as Tile;
        }
        float QuickMaths(float val, float min)
        {
            if (val == 0)
                return 0;

            float value = (val - min) / (1 - min);
            return value;
        }
        bool IsColor(Color color1, Color color2)
        {
            return (color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b);
        }
    }
}