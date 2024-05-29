using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportPoint;
    private Transform currentTeleportPoint;

    private void Start()
    {
        currentTeleportPoint = teleportPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentTeleportPoint != null)
            {
                other.transform.parent.position = currentTeleportPoint.position;
            }
        }
    }

    public void UpdateTeleportPoint(Transform newTeleportPoint)
    {
        currentTeleportPoint = newTeleportPoint;
    }
}
