using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    //Current weapon params
    public new string name;

    public enum WeaponType { Melee, Ranged };
    public WeaponType type;

    public Sprite sprite;

    public float weaponCooldown = 1;
    public float weaponSpeed = 1;
    public float weaponDamage = .5f;
    public float weaponKnockback = 8;
    public float mineSpeed = 5;

    [Header("For ranged weapons only:")]
    public Sprite bulletSprite;
    public float bulletSpeed;
}
