using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Terrain Type", menuName = "ScriptableObjects/Terrain Type")]
public class TerrainType : ScriptableObject
{
    public new string name;
    public Tile tile;
    public Color value;
    public float timeToBreak;
    public StoneRender render;
    public Item item;
}
