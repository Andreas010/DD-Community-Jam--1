using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Potential Script name: I_hate_myself
namespace DD_JAM.LevelGeneration
{
    public class ChunkGeneration : MonoBehaviour
    {
        public static ChunkGeneration instance;

        public Transform objectToCheck;
        public GameObject chunk;

        public GameObject[] publicCurrentChunks;

        public Dictionary<Vector2Int, TerrainType[,]> savedChunks;
        public Dictionary<Vector2Int, GameObject> currentChunks;

        private Vector2Int lastChunk = new Vector2Int(int.MaxValue, int.MaxValue);
        private bool firstFrame;

        public TMP_Text playerPosText;
        private bool isGenerating;

        [HideInInspector]
        public Vector2Int[] chunkPositions = {
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(0, 0),
            };

        public BossRoomType[] rooms;
        [Range(0f, 100f)]
        public float maskApplyanceChance;

        public bool playerIsInBossChunk;

        void Awake() => instance = this;

        void Start()
        {
            savedChunks = new Dictionary<Vector2Int, TerrainType[,]>();
            currentChunks = new Dictionary<Vector2Int, GameObject>();
            GetComponent<MapManager>().Init();
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F3))
                playerPosText.gameObject.SetActive(!playerPosText.gameObject.activeInHierarchy);

            Vector2Int currentChunk = new Vector2Int((int)((objectToCheck.position.x + (objectToCheck.position.x > 0 ? 25f : -25f)) / 50f), (int)((objectToCheck.position.y + (objectToCheck.position.y > 0 ? 25f : -25f)) / 50f));

            if (playerPosText.gameObject.activeInHierarchy)
            {
                playerPosText.text = $"X:{currentChunk.x:00} Y:{currentChunk.y:00} x:{(objectToCheck.position.x - (currentChunk.x * 50)):00.00} y:{(objectToCheck.position.y - (currentChunk.y * 50)):00.00} FPS:{1/Time.deltaTime:00.00}";
            }

            if (currentChunk != lastChunk && !isGenerating)
            {
                isGenerating = true;

                Rigidbody2D objRb = objectToCheck.GetComponent<Rigidbody2D>();
                Vector2 position = objRb.position;
                Vector2 speed = objRb.velocity;
                objRb.bodyType = RigidbodyType2D.Static;

                for (int i = 0; i < chunkPositions.Length; i++)
                {
                    if (!firstFrame)
                        continue;

                    TerrainType[,] chunkData;
                    GameObject chunkToDelete;
                    Vector2Int key = chunkPositions[i] + lastChunk;
                    currentChunks.TryGetValue(key, out chunkToDelete);

                    if (chunkToDelete != null)
                    {
                        chunkData = chunkToDelete.GetComponent<TileMapGenerator>().GetLevel();
                        if (savedChunks.ContainsKey(key))
                            savedChunks[key] = chunkData;
                        else
                            savedChunks.Add(key, chunkData);
                        if (chunkToDelete.GetComponent<BossRoomThinker>() != null)
                        {
                            chunkToDelete.GetComponent<BossRoomThinker>().canUpdate = false;
                            chunkToDelete.GetComponent<BossRoomThinker>().StopThinking();
                        }
                        chunkToDelete.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>().enabled = false;
                        chunkToDelete.GetComponent<UnityEngine.Tilemaps.TilemapRenderer>().enabled = false;
                        Destroy(chunkToDelete, i);
                        currentChunks.Remove(key);
                    }
                }

                publicCurrentChunks = new GameObject[chunkPositions.Length];

                //Generate new chunk
                for (int i = 0; i < chunkPositions.Length; i++)
                {
                    Vector3 pos = new Vector3((currentChunk.x + chunkPositions[i].x) * 50, (currentChunk.y + chunkPositions[i].y) * 50);
                    Vector2Int key = chunkPositions[i] + currentChunk;
                    GameObject newChunk = Instantiate(chunk, pos, Quaternion.identity);
                    newChunk.transform.parent = transform;

                    System.Random r = new System.Random(key.x * key.y);

                    TileMapGenerator tmg = newChunk.GetComponent<TileMapGenerator>();

                    if (r.Next(0, 100000) / 1000f <= maskApplyanceChance)
                        tmg.isChosen = true;

                    tmg.cg = this;
                    tmg.localPos = chunkPositions[i];
                    tmg.isPlayerChunk = chunkPositions[i] == new Vector2Int(0, 0);
                    tmg.allowBorders = false;

                    playerIsInBossChunk = tmg.isChosen && tmg.isPlayerChunk;

                    if (savedChunks.ContainsKey(key))
                    {
                        tmg.hasGenerated = true;
                        newChunk.GetComponent<LevelGenerator>().Generate();
                        tmg.ReGenerate(savedChunks[key]);
                    }
                    else
                    {
                        newChunk.GetComponent<LevelGenerator>().Generate();
                    }

                    newChunk.name = "CHUNK: " + key.x + "/" + key.y;

                    if (currentChunks.ContainsKey(key))
                        currentChunks[key] = newChunk;
                    else
                        currentChunks.Add(key, newChunk);

                    publicCurrentChunks[i] = newChunk;

                    firstFrame = true;
                }

                for (int i = 0; i < chunkPositions.Length; i++)
                {
                    GameObject newChunk = publicCurrentChunks[i];
                    TileMapGenerator tmg = newChunk.GetComponent<TileMapGenerator>();
                    tmg.allowBorders = true;
                    tmg.Generate();
                }

                lastChunk = currentChunk;
                objRb.bodyType = RigidbodyType2D.Dynamic;
                objRb.position = position;
                objRb.velocity = speed;

                isGenerating = false;
            }
        }
    }
}