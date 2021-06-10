using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DD_JAM.LevelGeneration
{
    public class ChunkGeneration : MonoBehaviour
    {
        public Transform objectToCheck;
        public GameObject chunk;

        public Dictionary<Vector2Int, GameObject> currentChunks;

        private Vector2Int lastChunk = new Vector2Int(100, 0);

        void Start()
        {
            currentChunks = new Dictionary<Vector2Int, GameObject>();
        }

        void LateUpdate()
        {
            Vector2Int[] chunkPositions = {
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

            Vector2Int currentChunk = new Vector2Int((int)(objectToCheck.position.x / 25f), (int)(objectToCheck.position.y / 25f));

            if(currentChunk != lastChunk)
            {
                //Something changed

                for (int i = 0; i < chunkPositions.Length; i++)
                {
                    GameObject chunkToDelete;
                    currentChunks.TryGetValue(chunkPositions[i] + currentChunk, out chunkToDelete);
                    if (chunkToDelete != null)
                        chunkToDelete.SetActive(false);
                }

                //currentChunks.Clear();

                for (int i = 0; i < chunkPositions.Length; i++)
                {
                    GameObject tmpChunk;
                    GameObject curChunk;
                    Vector3 pos = new Vector3((currentChunk.x + chunkPositions[i].x) * 50, (currentChunk.y + chunkPositions[i].y) * 50);
                    if (currentChunks.TryGetValue(currentChunk + chunkPositions[i], out tmpChunk))
                    {
                        curChunk = Instantiate(tmpChunk, pos, Quaternion.identity);
                    } else
                    {
                        curChunk = Instantiate(chunk, pos, Quaternion.identity);
                        currentChunks.Add(currentChunk + chunkPositions[i], curChunk);
                    }

                    curChunk.GetComponent<LevelGenerator>().enabled = true;
                    curChunk.GetComponent<Grid>().enabled = true;
                    curChunk.GetComponent<Tilemap>().enabled = true;
                    curChunk.GetComponent<TilemapCollider2D>().enabled = true;
                    curChunk.GetComponent<TilemapRenderer>().enabled = true;

                    curChunk.GetComponent<TileMapGenerator>().Refresh();
                    curChunk.transform.parent = transform;
                }
            }

            lastChunk = currentChunk;
        }
    }
}