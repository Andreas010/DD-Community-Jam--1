using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DD_JAM.LevelGeneration
{
    [RequireComponent(typeof(TileMapGenerator))]
    public class LevelGenerator : MonoBehaviour
    {
        public int seed;

        [Range(0, 1)]
        public float threshhold;
        public float newThreshhold;

        private float[,] level;

        public void Generate()
        {
            level = new float[50, 50];
            //r = new System.Random(seed);

            //float posX = r.Next(0, 1000000); // 726243
            //float posY = r.Next(0, 1000000); // 817325

            float posX = 726243;
            float posY = 817325;

            newThreshhold = Mathf.Clamp(threshhold + Mathf.PerlinNoise(transform.position.x / 250, transform.position.y / 250) - 0.5f, .4f, .6f);

            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    //float value = Mathf.PerlinNoise((((float)x / size.x * noiseScale.x) + posX) + (round ? (int)transform.position.x : transform.position.x) / size.x * noiseScale.x, (((float)y / size.y * noiseScale.y) + posY) + (round ? (int)transform.position.y : transform.position.y) / size.y * noiseScale.y);
                    float value = Mathf.PerlinNoise((float)x / 5 + posX + (int)transform.position.x / 5, (float)y / 5 + posY + (int)transform.position.y / 5);

                    level[x, y] = (value > threshhold ? value : 0f);
                }
            }

            GetComponent<TileMapGenerator>().Build(level, newThreshhold);
            GetComponent<TileMapGenerator>().Generate();
        }
    }
}