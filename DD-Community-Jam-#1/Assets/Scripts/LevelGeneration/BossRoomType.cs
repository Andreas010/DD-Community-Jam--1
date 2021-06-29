using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossRoomType
{
    public string name;
    public Texture2D texture;
    public IBossChunkable chunkThink;
}