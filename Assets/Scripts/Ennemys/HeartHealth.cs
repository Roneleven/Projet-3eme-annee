using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    public float destroySpeed;
    public Transform[] teleportPositions;
    private int currentTeleportIndex = 0;
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;
    [SerializeField] private List<TeleportPointBoxSpawnerPair> teleportPointBoxSpawnerPairs = new List<TeleportPointBoxSpawnerPair>();
    private FMOD.Studio.EventInstance Idle;

    public GameObject parent;
    public GameObject eyeRadius;
    public float moveSpeed = 5f;
    private Vector3 targetPosition;

    public VisualEffect HeartHit;
    public Transform HeartHitSpawnPoint;
    private VisualEffect HeartHitInstance;

    // Nouvelle variable pour stocker les points de téléportation accessibles après chaque téléportation
    public List<int> accessibleTeleportPoints = new List<int>();

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
        InitializeAccessibleTeleportPoints();
        Idle = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Behaviours/Idle");
        Idle.start();

        SetTargetForTeleportIndex(currentTeleportIndex);
    }

    void Update()
    {
        Idle.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        MoveToTarget();

        // Suppression de l'appel à SetRandomTarget()
    }

    void SetTargetForTeleportIndex(int teleportIndex)
    {
        if (teleportIndex >= 0 && teleportIndex < teleportPositions.Length)
        {
            // Sélectionne le point de téléportation en fonction de l'index spécifié
            Transform nextTeleportPosition = teleportPositions[teleportIndex];
            parent.transform.position = nextTeleportPosition.position;
            targetPosition = nextTeleportPosition.position;
        }
    }

    void MoveToTarget()
    {
        // Déplacer l'objet vers la position cible avec une vitesse constante
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void InitializeAccessibleTeleportPoints()
    {
        // Initialiser la liste des points de téléportation accessibles au début
        accessibleTeleportPoints.Clear();
        for (int i = 0; i < teleportPositions.Length; i++)
        {
            accessibleTeleportPoints.Add(i);
        }
    }

    public void TakeDamage(int damage)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Hitmarker");
        health -= damage;
        HeartHitInstance = Instantiate(HeartHit, HeartHitSpawnPoint.position, HeartHitSpawnPoint.rotation);
        HeartHitInstance.Play();
        HeartHitInstance.gameObject.AddComponent<VFXAutoDestroy>();

        if (health <= 0)
        {
            Idle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            TeleportHeart();
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");
            Idle.start();
        }
    }

    private void TeleportHeart()
    {
        DestroyCubesBeforeTeleport();
        DeactivateLinkedBoxSpawners();
        currentTeleportIndex++;
        if (currentTeleportIndex >= teleportPositions.Length)
        {
            currentTeleportIndex = 0; // ici c'est pour reset le tableau des TP s'il est passé par tout, à supprimer peut être pour la fin
        }
        SetTargetForTeleportIndex(currentTeleportIndex);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Locomotion/Teleport");

        if (accessibleTeleportPoints.Count > 0)
        {
            // Suppression de la sélection aléatoire du prochain point de téléportation

            lastTeleportIndex = currentTeleportIndex;
            Transform nextTeleportPosition = teleportPositions[lastTeleportIndex];
            parent.transform.position = nextTeleportPosition.position;
            targetPosition = nextTeleportPosition.position;

            health = maxHealth;

            if (heartSpawner != null)
            {
                heartSpawner.ChangePalierOnTeleport();
            }

            foreach (var pair in teleportPointBoxSpawnerPairs)
            {
                if (pair.teleportPointIndex == lastTeleportIndex)
                {
                    if (pair.boxSpawners != null)
                    {
                        foreach (var boxSpawner in pair.boxSpawners)
                        {
                            if (boxSpawner != null)
                            {
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
                if (pair.teleportPointIndex == lastTeleportIndex)
                {
                    if (pair.boxSpawnersNoHP != null)
                    {
                        foreach (var boxSpawnerNoHP in pair.boxSpawnersNoHP)
                        {
                            if (boxSpawnerNoHP != null)
                            {
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
                    boxSpawnerNoHP.StopAllCoroutines();
                }
            }
        }
    }

    private IEnumerator DestroyCubesGradually(GameObject[] cubes, float delay)
    {
        foreach (var cube in cubes)
        {
            Destroy(cube);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator DestroyPercentageOfCubesGradually(GameObject[] cubes, float percentageToRemove, float delay)
    {
        int cubesToRemove = Mathf.CeilToInt(cubes.Length * percentageToRemove);
        for (int i = 0; i < cubesToRemove; i++)
        {
            Destroy(cubes[i]);
            yield return new WaitForSeconds(delay);
        }
    }

    private void DestroyCubesBeforeTeleport()
    {
        GameObject[] heartGeneratedCubes = GameObject.FindGameObjectsWithTag("HeartBlock");
        int cubesToDestroy = Mathf.CeilToInt(heartGeneratedCubes.Length * 2f);
        StartCoroutine(DestroyCubesGradually(heartGeneratedCubes, destroySpeed / cubesToDestroy));

        foreach (var pair in teleportPointBoxSpawnerPairs)
        {
            if (pair.teleportPointIndex == lastTeleportIndex)
            {
                if (pair.boxSpawners != null)
                {
                    foreach (var boxSpawner in pair.boxSpawners)
                    {
                        GameObject[] generatedCubes = GameObject.FindGameObjectsWithTag("Block");
                        float percentageToRemove = 0.9f;
                        float delayBetweenCubes = 3f / generatedCubes.Length;
                        StartCoroutine(DestroyPercentageOfCubesGradually(generatedCubes, percentageToRemove, delayBetweenCubes));
                    }
                }
            }
        }

        foreach (var pair in teleportPointBoxSpawnerPairs)
        {
            if (pair.teleportPointIndex == lastTeleportIndex)
            {
                if (pair.boxSpawnersNoHP != null)
                {
                    foreach (var boxSpawnerNoHP in pair.boxSpawnersNoHP)
                    {
                        GameObject[] generatedCubes = GameObject.FindGameObjectsWithTag("Block");
                        float percentageToRemove = 0.9f;
                        float delayBetweenCubes = 3f / generatedCubes.Length;
                        StartCoroutine(DestroyPercentageOfCubesGradually(generatedCubes, percentageToRemove, delayBetweenCubes));
                    }
                }
            }
        }
    }

    public Transform getCurrentTeleportPoint()
    {
        // Assurez-vous que lastTeleportIndex est valide
        if (lastTeleportIndex >= 0 && lastTeleportIndex < teleportPositions.Length)
        {
            // Renvoie le transform du point de téléportation correspondant
            return teleportPositions[lastTeleportIndex];
        }
        else
        {
            // S'il n'y a pas de dernier index de téléportation valide, renvoie null
            return null;
        }
    }
}
