using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveInRange : MonoBehaviour
{

    [SerializeReference] GameObject objectToActivify;
    [SerializeReference] Transform distanceObject;
    [SerializeReference] float maxDistance;
    [System.NonSerialized] public float distance;

    [SerializeReference] bool usePlayerPosAsDistanceObject;

    void Start()
    {
        if (usePlayerPosAsDistanceObject)
            distanceObject = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, distanceObject.position);
        objectToActivify.SetActive(distance < maxDistance);
    }
}
