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

    private Vector2Int rectSmall = Vector2Int.zero;
    private Vector2Int rectBig = Vector2Int.zero;

    public RawImage mapImage;
    private Texture2D map;

    public void Init()
    {
        map = new Texture2D(1, 1);
        map.filterMode = FilterMode.Point;
        PlaceChunk(new Vector2Int(0, 0), ChunkType.Explored);
    }

    public void PlaceChunk(Vector2Int pos, ChunkType type)
    {
        if (pos.x < rectSmall.x)
            rectSmall.x = pos.x;
        if (pos.y < rectSmall.y)
            rectSmall.y = pos.y;

        if (pos.x > rectBig.x)
            rectBig.x = pos.x;
        if (pos.y > rectBig.y)
            rectBig.y = pos.y;


    }
}
