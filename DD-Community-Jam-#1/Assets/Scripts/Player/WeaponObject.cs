using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{

    PlayerMovement pm;
    bool canAtk = true;
    public Inventory inventory;

    private void Start()
    {
        pm = transform.parent.parent.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && canAtk && inventory.CanAttack())
        {
            canAtk = false;
            collision.GetComponent<Rigidbody2D>().AddForce((collision.gameObject.transform.position - transform.position) * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Enemy>().TakeKnockback();
            collision.GetComponent<Enemy>().UpdateHealth(-pm.weapon.weaponDamage);
            Invoke("CanAtkTrue", .2f);
        }
    }

    void CanAtkTrue()
    {
        canAtk = true;
    }

}
