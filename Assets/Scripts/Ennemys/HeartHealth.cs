using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TeleportPointBoxSpawnerPair
{
    public int teleportPointIndex;
    public List<BoxSpawner> boxSpawners;
    public List<BoxSpawnerNoHP> boxSpawnersNoHP;
}

public class HeartHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Transform[] teleportPositions;
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;
    [SerializeField] private List<TeleportPointBoxSpawnerPair> teleportPointBoxSpawnerPairs = new List<TeleportPointBoxSpawnerPair>();

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
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Locomotion/Teleport");


            health = maxHealth;

            if (heartSpawner != null)
            {
                heartSpawner.ChangePalierOnTeleport();
            }

            foreach (var pair in teleportPointBoxSpawnerPairs)
            {
                if (pair.teleportPointIndex == newTeleportIndex)
                {
                    if (pair.boxSpawners != null)
                    {
                        foreach (var boxSpawner in pair.boxSpawners)
                        {
                            if (boxSpawner != null)
                            {
                                //boxSpawner.gameObject.SetActive(true);
                                boxSpawner.StartCoroutine(boxSpawner.SpawnCube());
                            }
                            else
                            {
                                Debug.LogError("boxSpawner is null in pair.boxSpawners");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("pair.boxSpawners is null");
                    }
                }
            }

            foreach (var pair in teleportPointBoxSpawnerPairs)
            {
                if (pair.teleportPointIndex == newTeleportIndex)
                {
                    if (pair.boxSpawnersNoHP != null)
                    {
                        foreach (var boxSpawnerNoHP in pair.boxSpawnersNoHP)
                        {
                            if (boxSpawnerNoHP != null)
                            {
                                //boxSpawnerNoHP.gameObject.SetActive(true);
                                boxSpawnerNoHP.StartCoroutine(boxSpawnerNoHP.SpawnCube());
                            }
                            else
                            {
                                Debug.LogError("boxSpawnerNoHP is null in pair.boxSpawners");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("pair.boxSpawnersNoHP is null");
                    }
                }
            }
            UpdateAccessibleTeleportPoints();
        }
    }

    private void UpdateAccessibleTeleportPoints()
    {
        accessibleTeleportPoints.Remove(lastTeleportIndex);

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
                    //boxSpawner.gameObject.SetActive(false);
                    boxSpawner.StopAllCoroutines();
                }
            }
        }

        foreach (var pair in teleportPointBoxSpawnerPairs)
        {
            if (pair.teleportPointIndex == lastTeleportIndex)
            {
                foreach (var boxSpawnerNoHP in pair.boxSpawnersNoHP)
                {
                    //boxSpawnerNoHP.gameObject.SetActive(false);
                    boxSpawnerNoHP.StopAllCoroutines();
                }
            }
        }
    }
}
