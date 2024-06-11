using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportUpdate : MonoBehaviour
{
    public Teleport teleportScript;
    public int teleportIndex; // Index du point de t�l�portation � activer

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            teleportScript.UpdateTeleportPoint(teleportIndex);
        }
    }
}
