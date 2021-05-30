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
    [Header("Round2Int")]
    public bool round;
    [Header("Draw Gizmos")]
    public bool gizmos;
    public bool extras;

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
                level[x, y] = Mathf.PerlinNoise((((float)x / size.x * noiseScale.x) + posX) + (round ? (int)transform.position.x : transform.position.x) / size.x * noiseScale.x, (((float)y / size.y * noiseScale.y) + posY) + (round ? (int)transform.position.y : transform.position.y) / size.y * noiseScale.y) > threshhold;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (level == null || !gizmos)
            return;

        Gizmos.color = Color.white;

        for (int x = 0; x < (int)(size.x * masterScale.x); x++)
        {
            for (int y = 0; y < (int)(size.y * masterScale.y); y++)
            {
                if(level[x, y])
                {
                    if(round)
                        Gizmos.DrawWireCube(new Vector3((int)(x + transform.position.x - size.x * masterScale.x / 2), (int)(y + transform.position.y - size.y * masterScale.y / 2)), Vector3.one);
                    else
                        Gizmos.DrawWireCube(new Vector3(x + transform.position.x - size.x * masterScale.x / 2, y + transform.position.y - size.y * masterScale.y / 2), Vector3.one);
                }
            }
        }

        if (!extras)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position, new Vector3(size.x * masterScale.x, size.y * masterScale.y, 1));
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y), 1);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector3(size.x * masterScale.x, size.y * masterScale.y, 1));
        Gizmos.DrawWireSphere(new Vector3((int)transform.position.x, (int)transform.position.y), 1);
    }
}
