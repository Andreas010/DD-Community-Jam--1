using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    Vector2 originalPos;

    public float lerpTime;
    public Vector2 offset;
    public float smoothing;
    public Transform camLight;

    bool zoomed = false;

    void FixedUpdate()
    {
        //transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, -10), lerpTime);
        if (player != null)
        {
            //sets position to players with smoothing
            Vector2 desiredPosition = (Vector2)player.transform.position + offset + (player.GetComponent<Rigidbody2D>().velocity / 5);
            Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, smoothing);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, -10);

            if (Input.GetKeyDown(KeyCode.F1))
                zoomed = !zoomed;

            if(zoomed)
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, 7, smoothing);
                camLight.localPosition = Vector3.Lerp(camLight.localPosition, new Vector3(-14, 6, -10), smoothing);
            }
            else
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, 5, smoothing);
                camLight.localPosition = Vector3.Lerp(camLight.localPosition, new Vector3(-10.5f, 4.5f, -10), smoothing);
            }
        }
    }

    public void CameraShake(float frequency)
    {  
        for (int i = 0; i < 30; i++)
        {
            originalPos = player.position;
            Vector2 newPos = originalPos + Random.insideUnitCircle * frequency;
            transform.position = new Vector3(newPos.x, newPos.y, -10);
        }
    }
}
