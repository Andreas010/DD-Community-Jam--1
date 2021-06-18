using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{

    Transform player;
    Rigidbody2D rig;
    Enemy enemyScript;
    [SerializeField] LayerMask groundLayer;

    float xVel;
    [SerializeField] float jumpForce;
    [SerializeField] float speed;
    [SerializeField] float minDistFromGround = 1;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>().transform;
        enemyScript = GetComponent<Enemy>();
    }

    void Update()
    {

        //Grounded
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
        #endregion

        if (!enemyScript.takingKnockback)
        {
            if (player.position.x > transform.position.x)
                xVel = speed;
            else if (player.position.x < transform.position.x)
                xVel = -speed;

            if (player.position.y > transform.position.y + 1 && IsGrounded() && Vector2.Distance(transform.position, player.position) < 7)
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            rig.velocity = new Vector2(xVel, rig.velocity.y);
        }

    }

}
