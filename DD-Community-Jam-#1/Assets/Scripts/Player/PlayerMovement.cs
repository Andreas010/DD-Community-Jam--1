using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    Rigidbody2D rig;
    SpriteRenderer sr;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Animator weaponAnimator;

    [Header("Parameters")]
    [SerializeField] Vector2 speeds; //speeds.x = normal speed speeds.y = running speed
    [SerializeField] float jumpForce = 16;
    float jumpTimeCounter;
    [SerializeField] float jumpTime = 0.2f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float downwardJumpForce = -3;
    [SerializeField] float minDistFromGround = 0.6f;

    //bools
    bool sprinting;
    bool isJumping;
    bool facingRight;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
        #region
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
        if(Input.GetButtonDown("Fire1"))
        {
            if (y < 0) weaponAnimator.SetTrigger("Down");
            else if (y > 0) weaponAnimator.SetTrigger("Up");
            else if (x > 0 || facingRight) weaponAnimator.SetTrigger("Right");
            else if (x < 0 || !facingRight) weaponAnimator.SetTrigger("Left");
        }
        #endregion
    }
}
