using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{

    PlayerMovement pm;
    bool canAtk = true;
    public Inventory inventory;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        pm = transform.parent.parent.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        sr.sprite = pm.weapon.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && canAtk && inventory.CanAttack())
        {
            canAtk = false;
            collision.GetComponentInParent<Rigidbody2D>().AddForce((collision.gameObject.transform.position - transform.position) * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponentInParent<Rigidbody2D>().AddForce(Vector2.up * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Enemy>().TakeKnockback();
            collision.GetComponentInParent<Enemy>().UpdateHealth(-pm.weapon.weaponDamage);
            Invoke("CanAtkTrue", .2f);
        }
    }

    void CanAtkTrue()
    {
        canAtk = true;
    }

}
