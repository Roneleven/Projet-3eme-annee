using System.Collections;
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
    public float destroySpeed;
    public Transform[] teleportPositions;
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;
    [SerializeField] private List<TeleportPointBoxSpawnerPair> teleportPointBoxSpawnerPairs = new List<TeleportPointBoxSpawnerPair>();
    private FMOD.Studio.EventInstance Idle;

    public GameObject parent;
    public GameObject eyeRadius;
    public float moveSpeed = 5f;
    private Vector3 targetPosition;

<<<<<<< Updated upstream
    // Nouvelle variable pour stocker les points de téléportation accessibles après chaque téléportation
=======
    public float patternRadius;
    public MonoBehaviour scriptToToggle;

>>>>>>> Stashed changes
    public List<int> accessibleTeleportPoints = new List<int>();

    // New variables for changing materials
    public List<GameObject> objectsToChangeMaterial;
    public List<Material> materials;
    private int currentMaterialIndex = 0;
    private int currentObjectIndex = 0;

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
        InitializeAccessibleTeleportPoints();
        Idle = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Behaviours/Idle");
        Idle.start();

        SetRandomTarget();
    }

    void Update()
    {
        Idle.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        MoveToTarget();

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTarget();
        }
<<<<<<< Updated upstream
=======

        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > patternRadius)
            {
                if (scriptToToggle != null)
                {
                    scriptToToggle.enabled = false;
                }
            }
            else
            {
                if (scriptToToggle != null)
                {
                    scriptToToggle.enabled = true;
                }
            }
        }
>>>>>>> Stashed changes
    }

    void SetRandomTarget()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = eyeRadius.transform.localScale.x * 1.5f;
        float verticalOffset = Random.Range(-radius, radius);

        Vector3 offset = new Vector3(Mathf.Cos(angle), verticalOffset, Mathf.Sin(angle)) * radius;
        targetPosition = eyeRadius.transform.position + offset;
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void InitializeAccessibleTeleportPoints()
    {
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

        if (accessibleTeleportPoints.Count > 0)
        {
            int newTeleportIndex;
            do
            {
                newTeleportIndex = accessibleTeleportPoints[Random.Range(0, accessibleTeleportPoints.Count)];
            } while (newTeleportIndex == lastTeleportIndex);

            lastTeleportIndex = newTeleportIndex;
            Transform nextTeleportPosition = teleportPositions[lastTeleportIndex];
            parent.transform.position = nextTeleportPosition.position;
            SetRandomTarget();
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

            // Change the material of the next object
            ChangeNextObjectMaterial();
        }
    }

    private void ChangeNextObjectMaterial()
    {
        if (materials.Count > 0 && objectsToChangeMaterial.Count > 0)
        {
            // Change the material of the current object
            GameObject objectToChange = objectsToChangeMaterial[currentObjectIndex];
            currentMaterialIndex = (currentMaterialIndex + 1) % materials.Count;
            objectToChange.GetComponent<Renderer>().material = materials[currentMaterialIndex];

            // Move to the next object in the list
            currentObjectIndex = (currentObjectIndex + 1) % objectsToChangeMaterial.Count;
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
                        float percentageToRemove = 0.6f; 
                        float delayBetweenCubes = 5f / generatedCubes.Length;
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
                        float percentageToRemove = 0.6f; 
                        float delayBetweenCubes = 5f / generatedCubes.Length;
                        StartCoroutine(DestroyPercentageOfCubesGradually(generatedCubes, percentageToRemove, delayBetweenCubes));
                    }
                }
            }
        }
    }

    public Transform getCurrentTeleportPoint()
    {
        if (lastTeleportIndex >= 0 && lastTeleportIndex < teleportPositions.Length)
        {
            return teleportPositions[lastTeleportIndex];
        }
        else
        {
            return null;
        }
    }
}
