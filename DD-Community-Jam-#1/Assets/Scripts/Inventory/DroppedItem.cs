using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Inventory inventory;
    public Item item;
    public int count;
    public float timeToPickUp;
    float curTime;

    void Start()
    {
        curTime = timeToPickUp;
    }

    void Update()
    {
        curTime -= Time.deltaTime;

        if (curTime <= 0f)
            curTime = 0f;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && curTime <= 0f)
        {
            inventory.AddItem(item, count);
            Destroy(transform.parent.gameObject);
        }
    }
}
