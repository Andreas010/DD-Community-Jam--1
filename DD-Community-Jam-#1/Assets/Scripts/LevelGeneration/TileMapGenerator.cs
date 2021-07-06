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
        public TerrainType forcedGround;
        public TerrainType airTile;
        [HideInInspector]
        public ChunkGeneration cg;
        private BossRoomThinker bossRoomThinker;

        private Tilemap tilemap;
        private Vector2Int levelSize;
        private TerrainType[,] curLevel;
        private float curThreshhold;
        public Vector2[] possibleEnemyPositions;
        public bool isChosen;
        public bool isPlayerChunk;

        public GameObject enemy;

        private void Update()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < 20)
                Instantiate(enemy, possibleEnemyPositions[Random.Range(0, possibleEnemyPositions.Length)], Quaternion.identity);
        }

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

            BossRoomType type = cg.rooms[Random.Range(0, cg.rooms.Length)];
            Texture2D mask = type.texture;
            if (!mask.isReadable)
                return;

            bossRoomThinker = GetComponent<BossRoomThinker>();

            if (isChosen)
                bossRoomThinker.bossThink = type.chunkThink;

            MapManager.ChunkType chunkType = MapManager.ChunkType.None;

            if (isChosen && !isPlayerChunk)
                chunkType = MapManager.ChunkType.BossSeen;
            else if (!isChosen && !isPlayerChunk)
                chunkType = MapManager.ChunkType.Seen;
            else if (isChosen && isPlayerChunk)
                chunkType = MapManager.ChunkType.BossExplored;
            else
                chunkType = MapManager.ChunkType.Explored;

            GetComponentInParent<MapManager>().PlaceChunk(new Vector2Int((int)(transform.position.x / 50f), (int)(transform.position.y / 50f)), chunkType);

            System.Random r = new System.Random((int)(transform.position.x * 45558 + transform.position.y * 1452));

            for (int x = 0; x < levelSize.x; x++)
            {
                for (int y = 0; y < levelSize.y; y++)
                {
                    Vector3Int pos = tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y));

                    if (!isChosen || transform.position == Vector3.zero)
                        tilemap.SetTile(pos, curLevel[x, y].tile);
                    else
                    {
                        float red = mask.GetPixel(x, y).r;
                        float green = mask.GetPixel(x, y).g;
                        //float blue = mask.GetPixel(x, y).b;

                        if (green != 0)
                            tilemap.SetTile(pos, forcedGround.tile);
                        else if (r.Next(0, 1000) / 1000f <= red)
                            tilemap.SetTile(pos, airTile.tile);
                        else
                            tilemap.SetTile(pos, curLevel[x, y].tile);
                    }
                }
            }

            GenerateEnemyPositions();

            if(isChosen)
            {
                /* BLUE CHANNEL COLO(u)RS
                 * BLUE = 255 = Boss Position
                 * BLUE = 200 = Hazard Position
                 * BLUE = 150 = Minion Position
                 */
                bossRoomThinker.StartThinking(GenerateBossPosition(mask), GenerateMinionPositions(mask), GenerateHazardPositions(mask));
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
        public TerrainType GetTile(int x, int y)
        {
            return curLevel[x, y];
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

        public Vector2[] GenerateEnemyPositions()
        {
            List<Vector2> positions = new List<Vector2>();

            for (int x = 0; x < levelSize.x; x++)
            {
                for (int y = 0; y < levelSize.y; y++)
                {
                    if (y >= levelSize.y - 1)
                        continue;

                    if (GetTile(x, y).name != "Air" && GetTile(x, y + 1).name == "Air")
                        positions.Add(new Vector2((x - (levelSize.x / 2)) + transform.position.x + 0.5f, ((y+1) - (levelSize.y / 2)) + transform.position.y + 0.5f));
                }
            }

            possibleEnemyPositions = positions.ToArray();
            return positions.ToArray();
        }
        
        public Vector2[] GenerateMinionPositions(Texture2D mask)
        {
            List<Vector2> pos = new List<Vector2>();

            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    if (mask.GetPixel(x, y).b * 255f == 150f)
                        pos.Add(new Vector2(x + transform.position.x, y + transform.position.y));
                }
            }

            return pos.ToArray();
        }
        public Vector2[] GenerateHazardPositions(Texture2D mask)
        {
            List<Vector2> pos = new List<Vector2>();

            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    if (mask.GetPixel(x, y).b * 255f == 200f)
                        pos.Add(new Vector2(x + transform.position.x, y + transform.position.y));
                }
            }

            return pos.ToArray();
        }
        public Vector2 GenerateBossPosition(Texture2D mask)
        {
            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    if(mask.GetPixel(x, y).b * 255f == 1f)
                        return new Vector2(x + transform.position.x, y + transform.position.y);
                }
            }

            Debug.LogError("NO VALID BOSS POSITION", gameObject);
            return new Vector2(float.NaN, float.NaN);
        }

        void OnDrawGizmosSelected()
        {
            if (possibleEnemyPositions == null)
                return;

            for (int i = 0; i < possibleEnemyPositions.Length; i++)
            {
                Gizmos.DrawWireSphere(possibleEnemyPositions[i], 0.5f);
            }
        }

        private void OnDestroy()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))   
            { if(go.GetComponent<SetActiveInRange>().distance >15) Destroy(go); }
        }
    }
}