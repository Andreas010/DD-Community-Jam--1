using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "StoneRenderer", menuName ="ScriptableObjects/Stone Renderer")]
public class StoneRender : ScriptableObject
{
    public Tile LU;
    public Tile MU;
    public Tile RU;

    public Tile LM;
    public Tile MM;
    public Tile RM;

    public Tile LB;
    public Tile MB;
    public Tile RB;

    public Tile VU;
    public Tile VM;
    public Tile VB;

    public Tile HL;
    public Tile HM;
    public Tile HR;

    public Tile ELU;
    public Tile ERU;
    public Tile ELB;
    public Tile ERB;

    public Tile SG;
    public bool canPlaceInAir;
    public TerrainType internalValue;
}
