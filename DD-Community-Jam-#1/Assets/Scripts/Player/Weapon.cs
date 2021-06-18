using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    //Current weapon params
    public string name;

    public enum weaponType { Melee, Ranged };
    public weaponType type;

    public Sprite sprite;

    public float weaponCooldown = 1;
    float weaponSpeed = 1;
    public float weaponDamage = .5f;
    public float weaponKnockback = 8;
    public float mineSpeed = 5;
}
