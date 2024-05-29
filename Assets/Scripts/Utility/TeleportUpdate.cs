using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportUpdate : MonoBehaviour
{
    public Transform newTeleportPoint;
    public Teleport teleportScript; // TeleportTrigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            teleportScript.UpdateTeleportPoint(newTeleportPoint);
        }
    }
}
