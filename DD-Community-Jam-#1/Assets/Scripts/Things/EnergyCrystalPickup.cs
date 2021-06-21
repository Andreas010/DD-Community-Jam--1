using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCrystalPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.energyCrystals++;
            pm.UpdateEnergyCrystalsDisplay();
            Destroy(gameObject);
        }
    }
}
