using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public enum ChunkType
    {
        None, Seen, Explored, BossSeen, BossExplored
    }

    [System.Serializable]
    public class TypeToColor
    {
        public ChunkType type;
        public Color color;
    }

    public TypeToColor[] types;

    public RawImage mapImage;
    private Texture2D map;

    private Dictionary<Vector2Int, ChunkType> chunks;
    private Dictionary<ChunkType, Color> translationTable;
    private Vector2Int lowest = new Vector2Int(-1, -1);
    private Vector2Int highest = new Vector2Int(1, 1);

    public void Init()
    {
        chunks = new Dictionary<Vector2Int, ChunkType>();
        translationTable = new Dictionary<ChunkType, Color>();
        map = new Texture2D(1, 1)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        mapImage.texture = map;
        for (int i = 0; i < types.Length; i++)
        {
            translationTable.Add(types[i].type, types[i].color);
        }
        PlaceChunk(new Vector2Int(0, 0), ChunkType.Explored);
    }

    ChunkType CalculateType(ChunkType oldType, ChunkType newType)
    {
        if (oldType == ChunkType.Explored && newType == ChunkType.Seen)
            return oldType;
        if (oldType == ChunkType.BossExplored && newType == ChunkType.BossSeen)
            return oldType;
        return newType;
    }

    public void PlaceChunk(Vector2Int pos, ChunkType type)
    {
        

        if (chunks.ContainsKey(pos))
            chunks[pos] = CalculateType(chunks[pos], type);
        else
            chunks.Add(pos, type);

        if (lowest.x > pos.x)
            lowest.x = pos.x;
        if (lowest.y > pos.y)
            lowest.y = pos.y;

        if (highest.x < pos.x)
            highest.x = pos.x;
        if (highest.y < pos.y)
            highest.y = pos.y;

        UpdateMap();
    }

    void UpdateMap()
    {
        map.Resize(highest.x - lowest.x + 1, highest.y - lowest.y + 1);

        for (int x = lowest.x; x <= highest.x; x++)
        {
            for (int y = lowest.y; y <= highest.y; y++)
            {
                ChunkType curType = ChunkType.None;
                Color color = translationTable[curType];
                chunks.TryGetValue(new Vector2Int(x, y), out curType);
                translationTable.TryGetValue(curType, out color);
                //Debug.Log(x + "/" + y + "|" + (x - lowest.x) + "/" + (y - lowest.y));
                map.SetPixel(x - lowest.x, y - lowest.y, color);
            }
        }

        map.Apply();
    }
}
