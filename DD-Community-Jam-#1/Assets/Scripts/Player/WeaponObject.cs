using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{

    PlayerMovement pm;
    bool canAtk = true;

    private void Start()
    {
        pm = transform.parent.parent.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && canAtk)
        {
            canAtk = false;
            Debug.Log(collision.GetComponent<Rigidbody2D>());
            collision.GetComponent<Rigidbody2D>().AddForce((collision.gameObject.transform.position - transform.position) * pm.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up * pm.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Enemy>().TakeKnockback();
            collision.GetComponent<Enemy>().UpdateHealth(-pm.weaponDamage);
            Invoke("CanAtkTrue", .2f);
        }
    }

    void CanAtkTrue()
    {
        canAtk = true;
    }

}
