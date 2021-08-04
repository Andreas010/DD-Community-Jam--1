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
        public StoneRender render;
        public StoneRender forceRender;
        public StoneRender oreRenderer;
        [Range(0, 1)]
        public float oreFrequency;
        public float oreNoiseScale;

        public TerrainType renderReplacement;
        public TerrainType[] types;
        public TerrainType forcedGround;
        public TerrainType airTile;
        [HideInInspector]
        public ChunkGeneration cg;
        private BossRoomThinker bossRoomThinker;
        public Vector2Int localPos;
        public bool allowBorders;

        private Tilemap tilemap;
        private Vector2Int levelSize;
        private TerrainType[,] curLevel;
        private float curThreshhold;
        public Vector2[] possibleEnemyPositions;
        public bool isChosen;
        public bool hasGenerated;
        public bool isPlayerChunk;

        public GameObject enemy;

        [Space]
        public Color bossColor;
        public Color forceColor;
        public Color enemyColor;
        public Color hazardColor;

        private void Update()
        {
            int enemyCount = 0;

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Enemy"))
                    enemyCount++;
            }

            if(enemyCount <= 5)
            {
                Vector2 newPos = Vector2.zero;

                do { newPos = possibleEnemyPositions[Random.Range(0, possibleEnemyPositions.Length)]; }
                while (Vector2.Distance(newPos, PlayerHealth.instance.gameObject.transform.position) < 10);

                Instantiate(enemy, newPos, Quaternion.identity, transform);
            }
        }

        public void ReGenerate(TerrainType[,] level)
        {
            curLevel = level;
            Generate(true);
        }

        public void Build(float[,] level, float threshhold)
        {
            if (types.Length == 0)
                return;

            levelSize = new Vector2Int(level.GetLength(0), level.GetLength(1));

            curLevel = new TerrainType[levelSize.x, levelSize.y];

            System.Random r1 = new System.Random((int)(transform.position.x + transform.position.y));

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

        public void Generate(bool mayGenerateBoss = false)
        {
            tilemap = GetComponent<Tilemap>();

            BossRoomType type = cg.rooms[Random.Range(0, cg.rooms.Length)];
            Texture2D mask = type.texture;
            if (!mask.isReadable)
                return;

            bossRoomThinker = GetComponent<BossRoomThinker>();

            if (isChosen)
                bossRoomThinker.bossThink = type.chunkThink;

            MapManager.ChunkType chunkType;

            if (isChosen && !isPlayerChunk)
                chunkType = MapManager.ChunkType.BossSeen;
            else if (!isChosen && !isPlayerChunk)
                chunkType = MapManager.ChunkType.Seen;
            else if (isChosen && isPlayerChunk)
                chunkType = MapManager.ChunkType.BossExplored;
            else if (isPlayerChunk)
                    chunkType = MapManager.ChunkType.Current;
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
                    {
                        if(curLevel[x, y].render == oreRenderer)
                        {
                            tilemap.SetTile(pos, CalculateTile(oreRenderer, x, y));
                        }
                        else
                            tilemap.SetTile(pos, CalculateTile(render, x, y));
                    }

                    else if (isChosen)
                    {
                        float red = mask.GetPixel(x, y).r;
                        float green = mask.GetPixel(x, y).g;

                        if (green != 0)
                        {
                            if(!hasGenerated)
                                curLevel[x, y] = forceRender.internalValue;
                            tilemap.SetTile(pos, CalculateTile(forceRender, x, y));
                        }
                        else if (r.Next(0, 1000) / 1000f <= red && !hasGenerated)
                        {
                            curLevel[x, y] = airTile;
                            tilemap.SetTile(pos, airTile.tile);
                        }
                        else
                        {
                            if (curLevel[x, y].render == oreRenderer)
                            {
                                tilemap.SetTile(pos, CalculateTile(oreRenderer, x, y));
                            }
                            else
                                tilemap.SetTile(pos, CalculateTile(render, x, y));
                        }
                    }
                }
            }

            GenerateEnemyPositions();

            if(isChosen && isPlayerChunk && mayGenerateBoss)
            {
                /* BLUE CHANNEL COLO(u)RS
                 * BLUE = 255 = Boss Position
                 * BLUE = 200 = Hazard Position
                 * BLUE = 150 = Minion Position
                 */
                bossRoomThinker.StartThinking(GenerateBossPosition(mask), GenerateMinionPositions(mask), GenerateHazardPositions(mask));
            }
        }

        Tile CalculateTile(StoneRender curRender, int x, int y)
        {
            TerrainType curTile = GetTile(x, y);
            if (curTile == airTile || curTile == null)
                return airTile.tile;

            bool MU = GetTile(x    , y + 1) != airTile;
            bool LM = GetTile(x - 1, y    ) != airTile;
            bool RM = GetTile(x + 1, y    ) != airTile;
            bool MB = GetTile(x    , y - 1) != airTile;

            bool LU = GetTile(x - 1, y + 1) != airTile;
            bool RU = GetTile(x + 1, y + 1) != airTile;
            bool LB = GetTile(x - 1, y - 1) != airTile;
            bool RB = GetTile(x + 1, y - 1) != airTile;

            if (MU && !LM && !RM && !MB)
                return curRender.VB;
            else if (!MU && LM && !RM && !MB)
                return curRender.HR;
            else if (MU && LM && !RM && !MB)
                return curRender.RB;
            else if (!MU && !LM && RM && !MB)
                return curRender.HL;
            else if (MU && !LM && RM && !MB)
                return curRender.LB;
            else if (!MU && LM && RM && !MB)
                return curRender.HM;
            else if (MU && LM && RM && !MB)
                return curRender.MB;
            else if (!MU && !LM && !RM && MB)
                return curRender.VU;
            else if (MU && !LM && !RM && MB)
                return curRender.VM;
            else if (!MU && LM && !RM && MB)
                return curRender.RU;
            else if (MU && LM && !RM && MB)
                return curRender.RM;
            else if (!MU && !LM && RM && MB)
                return curRender.LU;
            else if (MU && !LM && RM && MB)
                return curRender.LM;
            else if (!MU && LM && RM && MB)
                return curRender.MU;
            else if (MU && LM && RM && MB)
            {
                if (LU && RU && LB && RB)
                    return curRender.MM;
                else if (!LU && RU && LB && RB)
                    return curRender.ERB;
                else if (LU && !RU && LB && RB)
                    return curRender.ELB;
                else if (LU && RU && !LB && RB)
                    return curRender.ERU;
                else if (LU && RU && LB && !RB)
                    return curRender.ELU;
                return curRender.MM;
            }
            else
                return curRender.SG;
        }

        public void SetTile(int x, int y, StoneRender renderer)
        {
            curLevel[x, y] = renderer.internalValue;
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y)), CalculateTile(renderer, x, y));

            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x - 1, y - (levelSize.y / 2) + transform.position.y)), CalculateTile(GetTile(x - 1, y).render, x - 1, y));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x + 1, y - (levelSize.y / 2) + transform.position.y)), CalculateTile(GetTile(x + 1, y).render, x + 1, y));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y - 1)), CalculateTile(GetTile(x, y - 1).render, x, y - 1));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x, y - (levelSize.y / 2) + transform.position.y + 1)), CalculateTile(GetTile(x, y + 1).render, x, y + 1));

            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x + 1, y - (levelSize.y / 2) + transform.position.y + 1)), CalculateTile(GetTile(x + 1, y + 1).render, x + 1, y + 1));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x + 1, y - (levelSize.y / 2) + transform.position.y - 1)), CalculateTile(GetTile(x + 1, y - 1).render, x + 1, y - 1));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x - 1, y - (levelSize.y / 2) + transform.position.y - 1)), CalculateTile(GetTile(x - 1, y - 1).render, x - 1, y - 1));
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(x - (levelSize.x / 2) + transform.position.x - 1, y - (levelSize.y / 2) + transform.position.y + 1)), CalculateTile(GetTile(x - 1, y + 1).render, x - 1, y + 1));
        }
        public TerrainType[,] GetLevel()
        {
            return curLevel;
        }
        public TerrainType GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= 50 || y >= 50)
                return airTile;

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
                    if (mask.GetPixel(x, y).b == enemyColor.b)
                        pos.Add(new Vector2(x + transform.position.x - 25, y + transform.position.y - 25));
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
                    if (mask.GetPixel(x, y).b == hazardColor.b)
                        pos.Add(new Vector2(x + transform.position.x - 25, y + transform.position.y - 25));
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
                    if(mask.GetPixel(x, y).b == bossColor.b)
                        return new Vector2(x + transform.position.x - 25, y + transform.position.y - 25);
                }
            }

            Debug.LogError("NO VALID BOSS POSITION", gameObject);
            return new Vector2(float.NaN, float.NaN);
        }

        private void OnDestroy()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy") == null)
                return;

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))   
            {
                if (go == null)
                    return;
                if (go.GetComponent<Enemy>() == null)
                    return;
                if (go.GetComponent<Enemy>().setActiveInRangeScript == null)
                    return;

                if(go.GetComponent<Enemy>().setActiveInRangeScript.distance > 20)
                    Destroy(go.GetComponent<Enemy>().objectToDestroy);
            }
        }
    }
}