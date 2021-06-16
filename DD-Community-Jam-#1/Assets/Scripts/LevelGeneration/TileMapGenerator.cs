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
        [System.Serializable]
        public class TerrainType
        {
            [Header("The tile to be placed")]
            public Tile tile;
            [Header("The color value of the Tile")]
            public Color value;
        }

        [Header("What color takes which spot?")]
        public Gradient colorLevel;
        [Header("What does exist?")]
        public TerrainType[] types;

        private Tilemap tilemap;
        private Vector2Int levelSize;
        private float[,] prevLevel;
        private float prevThreshhold;

        public void ReGenerate(float[,] level)
        {
            Generate(level, prevThreshhold);
            prevLevel = level;
        }

        public void Refresh()
        {
            Generate(prevLevel, prevThreshhold);
        }

        public void Generate(float[,] level, float threshhold)
        {
            prevLevel = level;
            prevThreshhold = threshhold;

            if (types.Length == 0)
                return;

            levelSize = new Vector2Int(level.GetLength(0), level.GetLength(1));
            tilemap = GetComponent<Tilemap>();

            for (int x = 0; x < levelSize.x; x++)
            {
                for (int y = 0; y < levelSize.y; y++)
                {
                    float value = level[x, y];

                    value = QuickMaths(value, threshhold);

                    if (types.Length == 1)
                    {
                        tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, x - (levelSize.x / 2) + transform.position.y)), types[0].tile);
                        continue;
                    }

                    Color tmpColor = colorLevel.Evaluate(value);
                    int index = 0xDEAD;

                    for (int i = 0; i < types.Length; i++)
                    {
                        if (IsColor(types[i].value, tmpColor))
                            index = i;
                    }

                    if (index != 0xDEAD)
                    {
                        tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), types[index].tile);
                    }
                }
            }
        }

        public void SetTile(int x, int y, Color tile)
        {
            Tile tmpTile = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (IsColor(types[i].value, tile))
                {
                    tmpTile = types[i].tile;
                }
            }

            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), tmpTile);
            prevLevel[x, y] = tile.r + tile.g + tile.b / 3f;
        }

        public float[,] GetLevel()
        {
            return prevLevel;
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