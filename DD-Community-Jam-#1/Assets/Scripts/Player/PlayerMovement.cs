using System.Collections;
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
    public LayerMask groundLayer;
    public Animator weaponAnimator;
    Animator animator;
    public Slider weaponCooldownSlider;
    public WeaponObject weaponScript;
    public GameObject eButton;
    public TextMeshProUGUI energyCrystalsText;
    public Inventory inventory;
    public GameObject bulletPrefab;
    PlayerHealth ph;

    [Header("Parameters")]
    public Vector2 speeds; //speeds.x = normal speed speeds.y = running speed
    Vector2 xtraVel; //extra velocities added to the player such as a dash or getting bumped when attacking
    public float jumpForce = 16;
    float jumpTimeCounter;
    public float jumpTime = 0.2f;
    public float fallMultiplier = 2.5f;
    public float downwardJumpForce = -3;
    public float minDistFromGround = 0.6f;
    float atkCooldown;
    public float dashForce;

    [HideInInspector] public Vector4 costForUpgrade = new Vector4(5, 5, 5, 5);

    [System.NonSerialized] public int energyCrystals = 5;

    //bools
    bool sprinting;
    bool isJumping;
    bool facingRight;
    bool canInteract;
    bool wasGrounded;
    bool canDash;

    //Current weapon params
    public Weapon weapon;
    [HideInInspector] public float weaponDamage = .5f;
    [HideInInspector] public float weaponKnockback = 8;

    public struct WeaponExtras
    {
        public float damage;
        public float cooldown;
        public float speed;
        public float knockdown;
        public float bulletSpeed;
        public float bulletDamage;
    }

    public Dictionary<Weapon, WeaponExtras> updateValues;

    void Start()
    {
        ph = GetComponent<PlayerHealth>();
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = transform.parent.GetComponent<Animator>();
        cam = transform.parent.GetComponentInChildren<Camera>().gameObject;

        updateValues = new Dictionary<Weapon, WeaponExtras>();

        canDash = true;
        
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
                if (wasGrounded == false && rig.velocity.y < -20)
                {
                    ph.ModifyHealth(-1);
                    ph.MakeInvincible(0.5f); 
                }
                wasGrounded = true;
                return true;
            }
            wasGrounded = false;
            return false;
        }
        bool IsTouchingCeiling()
        {
            Vector2 position = transform.position;
            Vector2 direction = Vector2.up;
            float distance = .5f ;

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

        //Dash
        #region
        if(Input.GetButtonDown("Dash") && canDash) 
        {
            Debug.Log("yuh");
            if(facingRight) xtraVel = new Vector2(xtraVel.x + dashForce, xtraVel.y);
            else xtraVel = new Vector2(xtraVel.x - dashForce, xtraVel.y);
            canDash = false;
            Invoke("DashCooldown", 1);
        }
        #endregion

        if (!inventory.isInventory && !ConsoleManager.instance.isConsole && !ShopManager.instance.isOpen)
        {
            sprinting = Input.GetButton("Sprint");
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x > 0) facingRight = true; else if (x < 0) facingRight = false;
            sr.flipX = !facingRight;

            //horizontal Movement
            if (!sprinting) rig.velocity = new Vector2(x * speeds.x, rig.velocity.y) + xtraVel; //speeds.x = normal speed
            else rig.velocity = new Vector2(x * speeds.y, rig.velocity.y) + xtraVel; //speeds.y = sprinting speed

            //jump
            if (IsGrounded() && (Input.GetButton("Jump")))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rig.velocity = new Vector2(rig.velocity.x, jumpForce) + xtraVel;
            }

            if ((Input.GetButton("Jump")) && isJumping == true)
            {
                if (!IsTouchingCeiling())
                {
                    float t_jumpforce = jumpForce;

                    if (jumpTimeCounter > 0)
                    {
                        rig.velocity = new Vector2(rig.velocity.x, t_jumpforce) + xtraVel;
                        jumpTimeCounter -= Time.deltaTime;
                        t_jumpforce -= Time.deltaTime;
                    }
                    else { isJumping = false; }
                }
                else { isJumping = false; if (rig.velocity.y > 0) { rig.velocity = new Vector2(rig.velocity.x, -downwardJumpForce) + xtraVel; } }

            }
            if (Input.GetButtonUp("Jump"))
            {
                isJumping = false;
                //rig.AddForce(Vector2.up * -5, ForceMode2D.Impulse);
                if (rig.velocity.y > 0)
                    rig.velocity = new Vector2(rig.velocity.x, -downwardJumpForce) + xtraVel;
            }

            if (rig.velocity.y < 0)
            {
                rig.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rig.velocity.y > 0 && !(Input.GetButton("Jump")))
            { rig.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; }
            #endregion

            if (xtraVel.x > 0) xtraVel.x -= Time.deltaTime * 5;
            if (xtraVel.y > 0) xtraVel.y -= Time.deltaTime * 5;
            if (xtraVel.x < 0) xtraVel.x += Time.deltaTime * 5;
            if (xtraVel.y < 0) xtraVel.y += Time.deltaTime * 5;
            if (xtraVel.x < 8.5f && xtraVel.x > 0) xtraVel.x = 0;
            if (xtraVel.y < 8.5f && xtraVel.x > 0) xtraVel.y = 0;
            if (xtraVel.x > -8.5f && xtraVel.x < 0) xtraVel.x = 0;
            if (xtraVel.y > -8.5f && xtraVel.x < 0) xtraVel.y = 0;

            //attack
            #region

            float addCooldown = 0;
            if (updateValues.ContainsKey(weapon))
                addCooldown = updateValues[weapon].cooldown;

            if (Input.GetButton("Fire1") && atkCooldown + addCooldown <= 0 && inventory.CanAttack() && !inventory.isInventory && !ConsoleManager.instance.isConsole)
            {
                //Set params
                //weaponCooldownSlider.maxValue = weapon.weaponCooldown;

                atkCooldown = weapon.weaponCooldown + addCooldown;

                float addSpeed = 0;
                if (updateValues.ContainsKey(weapon))
                    addSpeed = updateValues[weapon].speed;

                weaponAnimator.speed = weapon.weaponSpeed + addSpeed;

                //Animation
                if (y < 0) { weaponAnimator.SetTrigger("Down"); if (rig.velocity.y < -8) xtraVel.y = 1.5f; else xtraVel.y = .7f; }
                else if (y > 0) weaponAnimator.SetTrigger("Up");
                else if (x > 0 || facingRight) weaponAnimator.SetTrigger("Right");
                else if (x < 0 || !facingRight) weaponAnimator.SetTrigger("Left");

                if(weapon.type == Weapon.WeaponType.Ranged)
                {
                    //GameObject b = Instantiate(bulletPrefab, weaponScript.gameObject.transform.position, Quaternion.identity);
                    //b.transform.parent = transform.parent;
                    bulletPrefab.GetComponent<SpriteRenderer>().sprite = weapon.bulletSprite;

                    float addBulletSpeed = 0;
                    if (updateValues.ContainsKey(weapon))
                        addBulletSpeed = updateValues[weapon].bulletSpeed;

                    bulletPrefab.GetComponent<Bullet>().speed = weapon.bulletSpeed + addBulletSpeed;

                    float addBulletDamage = 0;
                    if (updateValues.ContainsKey(weapon))
                        addBulletDamage = updateValues[weapon].bulletDamage;

                    bulletPrefab.GetComponent<Bullet>().damage = weapon.bulletDamage + addBulletDamage;

                    GameObject b = Instantiate(bulletPrefab, weaponScript.gameObject.transform.position, Quaternion.identity);
                    b.transform.parent = transform.parent;
                }
            }

            weaponScript.enabled = weaponAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";
            weaponScript.GetComponent<BoxCollider2D>().enabled = weaponAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";

            if (atkCooldown > 0) atkCooldown -= Time.deltaTime;
            else if (atkCooldown < 0) atkCooldown = 0;

            //weaponCooldownSlider.value = atkCooldown;
            //if (weaponCooldownSlider.value <= 0) { Color c = weaponCooldownSlider.image.color; weaponCooldownSlider.image.color = new Vector4(c.r, c.g, c.b, c.a - .1f); }
            //else { Color c = weaponCooldownSlider.image.color; weaponCooldownSlider.image.color = new Vector4(c.r, c.g, c.b, 100); }
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

    void DashCooldown()
    {
        canDash = true;
    }

    public void UpgradeDamage(float value) => UpgradeWeapon(value, 0, 0, 0);
    public void UpgradeCooldown(float value) => UpgradeWeapon(0, value, 0, 0);
    public void UpgradeSpeed(float value) => UpgradeWeapon(0, 0, value, 0);
    public void UpgradeKnockback(float value) => UpgradeWeapon(0, 0, 0, value);

    public void UpgradeWeapon(float addDmg, float addCooldown, float addSpeed, float addKnockback)
    {
        if (updateValues.ContainsKey(weapon))
        {
            WeaponExtras stats = updateValues[weapon];

            stats.damage += addDmg;
            stats.cooldown += addCooldown;
            stats.speed += addSpeed;
            stats.knockdown += addKnockback;
            if (stats.cooldown < .15f) stats.cooldown += .15f;
        } else
        {
            WeaponExtras stats = new WeaponExtras();

            updateValues.Add(weapon, stats);
        }


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
