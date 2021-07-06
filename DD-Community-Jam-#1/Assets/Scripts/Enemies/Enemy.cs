using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    public Slider healthBar;
    public GameObject hitParticles;
    Rigidbody2D rig;
    public PhysicsMaterial2D[] physicsMaterials; // i 0 = no friction i 1 = has friction
    public GameObject energyCrystalPickup;

    public float chanceToDropEnergyCrystal;
    public float maxEnergyCrystalDropAmt;
    float health;
    public float maxHealth;
    float sliderA; //alpha colour

    [System.NonSerialized] public bool takingKnockback;
    bool canStopKnockback;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    private void Update()
    {
        if (takingKnockback) rig.sharedMaterial = physicsMaterials[1];
        else rig.sharedMaterial = physicsMaterials[0];

        if (rig.velocity.magnitude < .1f && takingKnockback && canStopKnockback) takingKnockback = false;

        if (sliderA > 0) sliderA -= Time.deltaTime * .3f;
        else if (sliderA < 0) sliderA = 0;
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            Color c = i.color;
            i.color = new Color(c.r, c.g, c.b, sliderA);
        }
    }

    public void UpdateHealth(float addHealth)
    {
        health += addHealth;
        if (health <= 0)
        {
            Destroy(transform.parent.gameObject);
            if( Mathf.RoundToInt(Random.Range(0, chanceToDropEnergyCrystal)) == 0)
            {
                for(int i = 0; i < Mathf.RoundToInt(Random.Range(1, maxEnergyCrystalDropAmt)); i++ )
                {
                    Instantiate(energyCrystalPickup, transform.position, Quaternion.identity);
                }
            }
        }
        sliderA = 1;
        healthBar.value = health;
        Instantiate(hitParticles, transform.position, transform.rotation);
        Camera.main.GetComponent<CameraScript>().CameraShake(1);
    }

    public void TakeKnockback()
    {
        takingKnockback = true;
        Invoke("CanStopKnockbackTrue", .2f);
    }

    public void CanStopKnockbackTrue()
    {
        canStopKnockback = true;
    }

}
