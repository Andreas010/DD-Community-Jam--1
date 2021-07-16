using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveInRange : MonoBehaviour
{

    public GameObject objectToActivify;
    public Transform distanceObject;
    public float maxDistance;
    [HideInInspector] public float distance;

    public bool usePlayerPosAsDistanceObject;

    void Start()
    {
        if (usePlayerPosAsDistanceObject)
            distanceObject = FindObjectOfType<PlayerMovement>().transform;
    }

    void LateUpdate()
    {
        distance = Vector2.Distance(transform.position, distanceObject.position);
        objectToActivify.SetActive(distance < maxDistance);
    }
}
