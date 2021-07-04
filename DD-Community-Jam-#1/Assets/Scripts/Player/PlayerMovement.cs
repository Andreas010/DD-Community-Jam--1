﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    GameObject cam;
    Rigidbody2D rig;
    SpriteRenderer sr;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Animator weaponAnimator;
    Animator animator;
    [SerializeField] Slider weaponCooldownSlider;
    [SerializeField] WeaponObject weaponScript;
    [SerializeField] Transform healthCanvas;
    [SerializeField] GameObject heartCanvasObject;
    [SerializeField] GameObject eButton;
    [SerializeField] TextMeshProUGUI energyCrystalsText;
    [SerializeField] Inventory inventory;

    [Header("Parameters")]
    [SerializeField] Vector2 speeds; //speeds.x = normal speed speeds.y = running speed
    [SerializeField] float jumpForce = 16;
    float jumpTimeCounter;
    [SerializeField] float jumpTime = 0.2f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float downwardJumpForce = -3;
    [SerializeField] float minDistFromGround = 0.6f;
    [SerializeField] float invincibleTime;
    float atkCooldown;
    float health = 3;
    float maxHealth = 3;

    [System.NonSerialized] public Vector4 costForUpgrade = new Vector4(5, 5, 5, 5);

    [System.NonSerialized] public int energyCrystals = 999;

    //bools
    bool sprinting;
    bool isJumping;
    bool facingRight;
    bool invincible;
    bool canInteract;

    //Current weapon params
    public Weapon weapon;
    float weaponCooldown = 1;
    float weaponSpeed = 1;
    [System.NonSerialized] public float weaponDamage = .5f;
    [System.NonSerialized] public float weaponKnockback = 8;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = transform.parent.GetComponent<Animator>();
        cam = transform.parent.GetComponentInChildren<Camera>().gameObject;

        UpdateHealthDisplay();
        UpdateEnergyCrystalsDisplay();
    }

    void Update()
    {
        #region
        bool IsGrounded()
        {
            Vector2 position = transform.position;
            Vector2 direction = Vector2.down;
            float distance = minDistFromGround;

            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
            if (hit.collider != null)
            {
                return true;
            }

            return false;
        }
        bool IsTouchingCeiling()
        {
            Vector2 position = transform.position;
            Vector2 direction = Vector2.up;
            float distance = minDistFromGround;

            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
            if (hit.collider != null)
            {
                return true;
            }
            return false;
        }
        #endregion
        ////////////////////////////////////////////////

        //movement
        #region
        if (!inventory.isInventory)
        {
            sprinting = Input.GetButton("Sprint");
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x > 0) facingRight = true; else if (x < 0) facingRight = false;
            sr.flipX = !facingRight;

            //horizontal Movement
            if (!sprinting) rig.velocity = new Vector2(x * speeds.x, rig.velocity.y); //speeds.x = normal speed
            else rig.velocity = new Vector2(x * speeds.y, rig.velocity.y); //speeds.y = sprinting speed

            //jump
            if (IsGrounded() && (Input.GetButton("Jump")))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rig.velocity = new Vector2(rig.velocity.x, jumpForce);
            }

            if ((Input.GetButton("Jump")) && isJumping == true)
            {
                if (!IsTouchingCeiling())
                {
                    float t_jumpforce = jumpForce;

                    if (jumpTimeCounter > 0)
                    {
                        rig.velocity = new Vector2(rig.velocity.x, t_jumpforce);
                        jumpTimeCounter -= Time.deltaTime;
                        t_jumpforce -= Time.deltaTime;
                    }
                    else { isJumping = false; }
                }
                else { isJumping = false; if (rig.velocity.y > 0) { rig.velocity = new Vector2(rig.velocity.x, -downwardJumpForce); } }

            }
            if (Input.GetButtonUp("Jump"))
            {
                isJumping = false;
                //rig.AddForce(Vector2.up * -5, ForceMode2D.Impulse);
                if (rig.velocity.y > 0)
                    rig.velocity = new Vector2(rig.velocity.x, -downwardJumpForce);
            }

            if (rig.velocity.y < 0)
            {
                rig.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rig.velocity.y > 0 && !(Input.GetButton("Jump")))
            { rig.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; }
            #endregion

            //attack
            #region
            if (Input.GetButton("Fire1") && atkCooldown <= 0 && inventory.CanAttack() && !inventory.isInventory)
            {
                //Set params
                weaponCooldownSlider.maxValue = weapon.weaponCooldown;
                atkCooldown = weapon.weaponCooldown;
                weaponAnimator.speed = weapon.weaponSpeed;

                //Animation
                if (y < 0) weaponAnimator.SetTrigger("Down");
                else if (y > 0) weaponAnimator.SetTrigger("Up");
                else if (x > 0 || facingRight) weaponAnimator.SetTrigger("Right");
                else if (x < 0 || !facingRight) weaponAnimator.SetTrigger("Left");
            }

            weaponScript.enabled = weaponAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";
            weaponScript.GetComponent<BoxCollider2D>().enabled = weaponAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";

            if (atkCooldown > 0) atkCooldown -= Time.deltaTime;
            else if (atkCooldown < 0) atkCooldown = 0;

            weaponCooldownSlider.value = atkCooldown;
            #endregion

            //animation
            #region
            animator.SetBool("Grounded", IsGrounded());
            animator.SetFloat("YVel", rig.velocity.y);
            animator.SetFloat("Magnitude", rig.velocity.magnitude);
            #endregion

            eButton.SetActive(canInteract);
        }
    }

    void UpdateHealthDisplay()
    {
        for (int i = 0; i < healthCanvas.transform.childCount; i++) 
        {
            Destroy(healthCanvas.transform.GetChild(i).gameObject); 
        }
        for (int i = 0; i < Mathf.RoundToInt(health); i++)
        { Instantiate( heartCanvasObject , healthCanvas); }
    }

    public void UpgradeHealth()
    {
        maxHealth++;
        health = maxHealth;
        UpdateHealthDisplay();
    }

    public void TakeDamage(float damage)
    {
        if (!invincible)
        {
            health -= damage;
            if (health <= 0) SceneManager.LoadScene("Died");
            cam.GetComponent<CameraScript>().CameraShake(1);
            invincible = true;
            Invoke("UnInvincible", invincibleTime);
            UpdateHealthDisplay();
        }
    }

    void UnInvincible()
    {
        invincible = false;
    }

    public void UpgradeWeapon(float addDmg, float addCooldown, float addSpeed, float addKnockback)
    {
        weapon.weaponDamage += addDmg;
        weapon.weaponCooldown += addCooldown;
        weapon.weaponSpeed += addSpeed;
        weapon.weaponKnockback += addKnockback;
        if (weapon.weaponCooldown < .15f) weapon.weaponCooldown = .15f;
        energyCrystals -= 5;
    }

    void ChangeWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        weaponScript.gameObject.GetComponent<SpriteRenderer>().sprite = weapon.sprite;
    }

    public void UpdateEnergyCrystalsDisplay()
    {
        energyCrystalsText.text = energyCrystals.ToString();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Interactable"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canInteract = false;
        }
    }

}
