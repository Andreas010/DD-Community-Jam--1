using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform player;
    Vector3 originalPos;

    public float lerpTime;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, -10), lerpTime);
    }

    public void CameraShake(float frequency)
    {
        
        for (int i = 0; i < 30; i++)
        {
            originalPos = player.position;
            transform.position = originalPos + Random.insideUnitSphere * frequency;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
