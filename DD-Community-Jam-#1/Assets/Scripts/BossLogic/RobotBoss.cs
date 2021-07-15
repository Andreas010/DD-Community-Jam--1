using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBoss : MonoBehaviour
{
    private Transform playerPositon;
    public Vector2 ranges;
    private bool isActivated;
    public bool isAttacking;
    public Collider2D fistCollider;
    public Collider2D footCollider;
    public Enemy enemyScript;

    public float speed;
    public float jumpStrengh;

    private Rigidbody2D rb;
    private Animator animator;

    public LayerMask levelLayer;
    public LayerMask playerLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPositon = FindObjectOfType<PlayerMovement>().transform;
        animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        if(!isActivated)
        {
            if (Vector2.Distance(playerPositon.position, GetCenter(transform.position)) < ranges.x)
                isActivated = true;
        } else
        {
            if (Vector2.Distance(playerPositon.position, GetCenter(transform.position)) > ranges.y)
                isActivated = false;
        }

        animator.SetBool("NearPlayer", isActivated);

        Vector2 newMove = rb.velocity;

        if (isActivated && !isAttacking && !enemyScript.takingKnockback)
        {
            //Run logic

            bool isRight = playerPositon.position.x > transform.position.x;
            newMove.x = isRight ? speed : -speed;

            transform.eulerAngles = new Vector3(0, isRight ? 0 : 180, 0);

            if (playerPositon.position.y > GetCenter(transform.position).y + 1.5f)
                newMove.y = jumpStrengh;

            if (Physics2D.Raycast(transform.position, playerPositon.position - transform.position, Vector2.Distance(transform.position, playerPositon.position), levelLayer) && playerPositon.position.y > transform.position.y)
                newMove.y = jumpStrengh;
        }
        else
            newMove.x = 0;

        if ((fistCollider.IsTouchingLayers(playerLayer) || footCollider.IsTouchingLayers(playerLayer)) && !isAttacking)
        {
            isAttacking = true;
            if (fistCollider.IsTouchingLayers(playerLayer))
                animator.SetTrigger("Punch");
            else if (footCollider.IsTouchingLayers(playerLayer))
                animator.SetTrigger("Kick");
        }

        if (isAttacking) //TODO: Fix the boss jumping, while attacking
            newMove.x = 0;

        rb.velocity = newMove;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCenter(transform.position), ranges.x);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetCenter(transform.position), ranges.y);
    }

    Vector2 GetCenter(Vector2 position)
    {
        return new Vector2(position.x, position.y + 1.5f);
    }
}
