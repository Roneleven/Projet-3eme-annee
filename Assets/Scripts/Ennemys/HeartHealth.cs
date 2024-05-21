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

    public List<int> accessibleTeleportPoints = new List<int>();

    // List of prefabs with destruction animations for each teleport position
    public List<GameObject> destructionAnimationPrefabs = new List<GameObject>();

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
    }

    void SetTargetForTeleportIndex(int teleportIndex)
    {
        if (teleportIndex >= 0 && teleportIndex < teleportPositions.Length)
        {
            Transform nextTeleportPosition = teleportPositions[teleportIndex];
            parent.transform.position = nextTeleportPosition.position;
            targetPosition = nextTeleportPosition.position;
        }
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
        HeartHitInstance = Instantiate(HeartHit, HeartHitSpawnPoint.position, HeartHitSpawnPoint.rotation);
        HeartHitInstance.Play();
        HeartHitInstance.gameObject.AddComponent<VFXAutoDestroy>();

        if (health <= 0)
        {
            Idle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            PlayDestructionAnimation(lastTeleportIndex);
            TeleportHeart();
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");
            Idle.start();
        }
    }

    private void PlayDestructionAnimation(int teleportIndex)
    {
        if (teleportIndex >= 0 && teleportIndex < destructionAnimationPrefabs.Count)
        {
            GameObject prefab = destructionAnimationPrefabs[teleportIndex];
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, teleportPositions[teleportIndex].position, Quaternion.identity);
                Animator animator = instance.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("Destroy");
                }
            }
        }
    }

    private void TeleportHeart()
    {
        DestroyCubesBeforeTeleport();
        DeactivateLinkedBoxSpawners();
        currentTeleportIndex++;
        if (currentTeleportIndex >= teleportPositions.Length)
        {
            currentTeleportIndex = 0;
        }
        SetTargetForTeleportIndex(currentTeleportIndex);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Locomotion/Teleport");

        if (accessibleTeleportPoints.Count > 0)
        {
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
