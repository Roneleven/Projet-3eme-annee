using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct TeleportPointBoxSpawnerPair
{
    public int teleportPointIndex;
    public List<BoxSpawner> boxSpawners;
}

public class HeartHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public int currentPalier = 1;
    public Transform[] teleportPositions; // Tableau des positions de téléportation
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;
    public List<TeleportPointBoxSpawnerPair> teleportPointBoxSpawnerPairs = new List<TeleportPointBoxSpawnerPair>();

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            TeleportHeart();
        }
    }

    private void TeleportHeart()
    {
        // Désactiver les BoxSpawner associés à l'index de téléportation actuel
        DeactivateLinkedBoxSpawners();

        if (teleportPositions.Length > 0)
        {
            int newTeleportIndex;
            do
            {
                newTeleportIndex = Random.Range(0, teleportPositions.Length);
            } while (newTeleportIndex == lastTeleportIndex);
            lastTeleportIndex = newTeleportIndex;
            Transform nextTeleportPosition = teleportPositions[lastTeleportIndex];
            transform.position = nextTeleportPosition.position;

            health = maxHealth;

            // Ajouter cette ligne pour déclencher le changement de palier
            if (heartSpawner != null)
            {
                heartSpawner.ChangePalierOnTeleport();
            }

            // Activer tous les BoxSpawner associés à l'index de téléportation
            foreach (var pair in teleportPointBoxSpawnerPairs)
            {
                if (pair.teleportPointIndex == newTeleportIndex)
                {
                    foreach (var boxSpawner in pair.boxSpawners)
                    {
                        //boxSpawner.gameObject.SetActive(true);
                        boxSpawner.StartCoroutine(boxSpawner.SpawnCube());
                    }
                }
            }
        }
    }

    private void DeactivateLinkedBoxSpawners()
    {
        foreach (var pair in teleportPointBoxSpawnerPairs)
        {
            if (pair.teleportPointIndex == lastTeleportIndex)
            {
                foreach (var boxSpawner in pair.boxSpawners)
                {
                    //boxSpawner.gameObject.SetActive(false);
                    boxSpawner.StopAllCoroutines();
                }
            }
        }
    }
}
