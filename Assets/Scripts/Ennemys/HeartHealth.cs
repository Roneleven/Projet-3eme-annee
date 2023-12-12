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
    public Transform[] teleportPositions;
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;
    private List<TeleportPointBoxSpawnerPair> teleportPointBoxSpawnerPairs = new List<TeleportPointBoxSpawnerPair>();

    // Nouvelle variable pour stocker les points de t�l�portation accessibles apr�s chaque t�l�portation
    public List<int> accessibleTeleportPoints = new List<int>();

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
        InitializeAccessibleTeleportPoints();
    }

    private void InitializeAccessibleTeleportPoints()
    {
        // Initialiser la liste des points de t�l�portation accessibles au d�but
        accessibleTeleportPoints.Clear();
        for (int i = 0; i < teleportPositions.Length; i++)
        {
            accessibleTeleportPoints.Add(i);
        }
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
        DeactivateLinkedBoxSpawners();

        if (accessibleTeleportPoints.Count > 0)
        {
            int newTeleportIndex;
            do
            {
                // Choisir un point de t�l�portation parmi ceux qui sont accessibles
                newTeleportIndex = accessibleTeleportPoints[Random.Range(0, accessibleTeleportPoints.Count)];
            } while (newTeleportIndex == lastTeleportIndex);

            lastTeleportIndex = newTeleportIndex;
            Transform nextTeleportPosition = teleportPositions[lastTeleportIndex];
            transform.position = nextTeleportPosition.position;

            health = maxHealth;

            if (heartSpawner != null)
            {
                heartSpawner.ChangePalierOnTeleport();
            }

            foreach (var pair in teleportPointBoxSpawnerPairs)
            {
                if (pair.teleportPointIndex == newTeleportIndex)
                {
                    foreach (var boxSpawner in pair.boxSpawners)
                    {
                        boxSpawner.gameObject.SetActive(true);
                    }
                }
            }

            // Mettre � jour la liste des points de t�l�portation accessibles apr�s cette t�l�portation
            UpdateAccessibleTeleportPoints();
        }
    }

    private void UpdateAccessibleTeleportPoints()
    {
        // Retirer le point de t�l�portation actuel de la liste des points accessibles
        accessibleTeleportPoints.Remove(lastTeleportIndex);

        // R�initialiser la liste des points accessibles si tous ont �t� visit�s
        if (accessibleTeleportPoints.Count == 0)
        {
            InitializeAccessibleTeleportPoints();
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
                    boxSpawner.gameObject.SetActive(false);
                }
            }
        }
    }
}
