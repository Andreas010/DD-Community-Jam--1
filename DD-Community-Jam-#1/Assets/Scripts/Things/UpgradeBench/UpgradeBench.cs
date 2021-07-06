using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeBench : MonoBehaviour
{

    public GameObject canvas;
    PlayerMovement pm;
    public TextMeshProUGUI[] costs;

    bool canInteract;

    private void Update()
    {
        if(canInteract && Input.GetButtonDown("Interact"))
        {
            canvas.SetActive(!canvas.activeSelf);
            if (!canvas.activeSelf)
            {
                for (int i = 0; i < 4; i++)
                {
                    costs[i].transform.parent.parent.gameObject.SetActive(false);
                }
            }
        }
        if(pm != null)
        {
            for(int i = 0; i < 4; i++)
            {
                costs[i].text = Mathf.RoundToInt(IToXYZW(i)).ToString();
            }
        }
    }

    float IToXYZW(int i)//converts an index to an x y z or w
    {
        if (i == 0) return pm.costForUpgrade.x;
        else if (i == 1) return pm.costForUpgrade.y;
        else if (i == 2) return pm.costForUpgrade.z;
        else if (i == 3) return pm.costForUpgrade.w;
        else return 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canInteract = true;
            pm = collision.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            canvas.SetActive(false);
            for(int i = 0; i < 4; i++)
            {
                costs[i].transform.parent.parent.gameObject.SetActive(false);
            }
        }
    }

    public void UpgradeHealth()
    {
        if (pm.energyCrystals >= pm.costForUpgrade.w)
        { pm.UpgradeHealth(); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.w); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.w += 2; }
    }

    public void UpgradeWeapon(string toUpgrade)
    {
        switch (toUpgrade)
        {
            case "Speed":
                if (pm.weapon.weaponCooldown > .15f && pm.energyCrystals >= pm.costForUpgrade.x)
                { pm.UpgradeWeapon(0, -.1f, 0, 0); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.x); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.x += 2; }
                break;
            case "Damage":
                if (pm.energyCrystals >= pm.costForUpgrade.y)
                { pm.UpgradeWeapon(.2f, 0, 0, 0); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.y); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.y += 2; }
                break;
            case "Knockback":
                if (pm.energyCrystals >= pm.costForUpgrade.z)
                { pm.UpgradeWeapon(0, 0, 0, 3); pm.energyCrystals -= Mathf.RoundToInt(pm.costForUpgrade.z); pm.UpdateEnergyCrystalsDisplay(); pm.costForUpgrade.z += 2; }
                break;
        }
    }

}
