using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGenerator : MonoBehaviour
{
    public int seed;

    System.Random r;

    [Range(0, 1)]
    public float threshhold;
    [Header("The Scale of everything")]
    public Vector2    masterScale;
    [Header("Array size")]
    public Vector2Int size;
    [Header("Noise scale")]
    public Vector2Int noiseScale;

    private bool[,] level;

    void Update()
    {
        level = new bool[(int)(size.x * masterScale.x), (int)(size.y * masterScale.y)];
        r = new System.Random(seed);

        float posX = r.Next(0, 1000000);
        float posY = r.Next(0, 1000000);

        for (int x = 0; x < (int)(size.x * masterScale.x); x++)
        {
            for (int y = 0; y < (int)(size.y * masterScale.y); y++)
            {
                level[x, y] = Mathf.PerlinNoise((((float)x / size.x * noiseScale.x) + posX) + transform.position.x / size.x * noiseScale.x, (((float)y / size.y * noiseScale.y) + posY) + transform.position.y / size.y * noiseScale.y) > threshhold;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (level == null)
            return;

        for (int x = 0; x < (int)(size.x * masterScale.x); x++)
        {
            for (int y = 0; y < (int)(size.y * masterScale.y); y++)
            {
                if(level[x, y])
                    Gizmos.DrawWireCube(new Vector3((int)(x + transform.position.x) - size.x * masterScale.x / 2, (int)(y + transform.position.y) - size.y * masterScale.y / 2), Vector3.one);
            }
        }
    }
}
