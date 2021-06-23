using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        None, Weapon, Block, Resource
    }

    public string displayName;
    public ItemType type;
    public int maxCount;
    public ScriptableObject scriptObject;
}
