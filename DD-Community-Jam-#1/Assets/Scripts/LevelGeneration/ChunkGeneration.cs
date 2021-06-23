﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Potential Script name: I_hate_myself
namespace DD_JAM.LevelGeneration
{
    public class ChunkGeneration : MonoBehaviour
    {
        public Transform objectToCheck;
        public GameObject chunk;

        public GameObject[] publicCurrentChunks;

        public Dictionary<Vector2Int, TerrainType[,]> savedChunks;
        public Dictionary<Vector2Int, GameObject> currentChunks;

        private Vector2Int lastChunk = new Vector2Int(int.MaxValue, int.MaxValue);
        private bool firstFrame;

        public Texture2D[] masks;
        [Range(0f, 100f)]
        public float maskApplyanceChance;

        void Start()
        {
            savedChunks = new Dictionary<Vector2Int, TerrainType[,]>();
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

            Vector2Int currentChunk = new Vector2Int((int)(objectToCheck.position.x / 50f), (int)(objectToCheck.position.y / 50f));

            if(currentChunk != lastChunk)
            {
                //Something changed

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
                        Destroy(chunkToDelete);
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

                    if (r.Next(0, 100000) / 1000f <= maskApplyanceChance)
                        newChunk.GetComponent<TileMapGenerator>().isChosen = true;

                    newChunk.GetComponent<TileMapGenerator>().cg = this;

                    if (savedChunks.ContainsKey(key))
                    {
                        //TODO: Find out why
                        newChunk.GetComponent<LevelGenerator>().Generate();
                        newChunk.GetComponent<TileMapGenerator>().ReGenerate(savedChunks[key]);
                    } else
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
            }

            lastChunk = currentChunk;
        }
    }
}