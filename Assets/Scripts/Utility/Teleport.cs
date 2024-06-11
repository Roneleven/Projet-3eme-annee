using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform[] teleportPoints; // Tableau de points de téléportation
    private Transform currentTeleportPoint;

    private void Start()
    {
        if (teleportPoints.Length > 0)
        {
            currentTeleportPoint = teleportPoints[0]; // Initialiser avec le premier point de téléportation
        }
        else
        {
            Debug.LogError("Aucun point de téléportation défini.");
        }
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

    public void UpdateTeleportPoint(int index)
    {
        if (index >= 0 && index < teleportPoints.Length)
        {
            currentTeleportPoint = teleportPoints[index];
        }
        else
        {
            Debug.LogError("Index de point de téléportation invalide.");
        }
    }
}
