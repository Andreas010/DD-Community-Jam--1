using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed;
    Rigidbody2D rig;
    PlayerMovement pm;
    public float damage;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        pm = transform.parent.GetComponentInChildren<PlayerMovement>();

        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        rig.AddForce(transform.right * 3, ForceMode2D.Impulse);

        Destroy(gameObject, 4);
    }

    void Update()
    {
        rig.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Debug.Log(collision.gameObject);
            Debug.Log(pm);
            collision.GetComponent<Enemy>().rig.AddForce((collision.gameObject.transform.position - transform.position) * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Enemy>().rig.AddForce(Vector2.up * pm.weapon.weaponKnockback, ForceMode2D.Impulse);
            collision.GetComponent<Enemy>().TakeKnockback();
            collision.GetComponentInParent<Enemy>().UpdateHealth(damage);
            Destroy(gameObject);
        }
    }

}
