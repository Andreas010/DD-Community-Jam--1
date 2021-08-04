using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeBench : MonoBehaviour
{
    public AnimationCurve hight;
    public AnimationCurve sizeX;
    public AnimationCurve sizeY;
    private float time;
    public float multiply;
#pragma warning disable CS0649
    private bool open;
#pragma warning restore CS0649

    public Transform popupObj;

    void Start()
    {
        time = 0;
        popupObj.localPosition = new Vector2(0, hight.Evaluate(0));
        popupObj.localScale = new Vector2(sizeX.Evaluate(0), sizeY.Evaluate(0));
    }

    void FixedUpdate()
    {
        time += Time.deltaTime * (open ? multiply : -multiply);
        time = Mathf.Clamp01(time);

        popupObj.localPosition = new Vector2(0, hight.Evaluate(time));
        popupObj.localScale = new Vector2(sizeX.Evaluate(time), sizeY.Evaluate(time));
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            ShopManager.instance.Open();
        }
    }

    //public void UpgradeHealth()
    //{
    //    if (pm.energyCrystals >= pm.costForUpgrade.w)
    //    { pm.UpgradeHealth(); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.w); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.w += 2; }
    //}
    //
    //public void UpgradeWeapon(string toUpgrade)
    //{
    //    switch (toUpgrade)
    //    {
    //        case "Speed":
    //            if (pm.weapon.weaponCooldown > .15f && pm.energyCrystals >= pm.costForUpgrade.x)
    //            { pm.UpgradeWeapon(0, -.1f, 0, 0); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.x); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.x += 2; }
    //            break;
    //        case "Damage":
    //            if (pm.energyCrystals >= pm.costForUpgrade.y)
    //            { pm.UpgradeWeapon(.2f, 0, 0, 0); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.y); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.y += 2; }
    //            break;
    //        case "Knockback":
    //            if (pm.energyCrystals >= pm.costForUpgrade.z)
    //            { pm.UpgradeWeapon(0, 0, 0, 3); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.z); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.z += 2; }
    //            break;
    //    }
    //}
}
