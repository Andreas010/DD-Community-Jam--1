using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ShopOption
{
    public string name;
    [TextArea(3, 3)]
    public string description;
    public int price;
    public int increaseOfPrice;
    public int maxBuys;
    public Texture2D image;
    public UnityEvent onBuy;
}
