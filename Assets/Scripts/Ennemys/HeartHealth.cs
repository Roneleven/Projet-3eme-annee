using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;

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
    private bool hasBeenDestroyed = false;
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
    public Animator doorAnimator;

    public VisualEffect HeartHit;
    public Transform HeartHitSpawnPoint;
    private VisualEffect HeartHitInstance;
    public UniversalRendererData urpRendererData;

    public List<int> accessibleTeleportPoints = new List<int>();
    public BossPatternManager bossPatternManager;
    public GameObject youWinPanel;

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
        InitializeAccessibleTeleportPoints();
        Idle = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Behaviours/Idle");
        Idle.start();

        SetRandomTarget();
        SetTargetForTeleportIndex(currentTeleportIndex);
        ActivateLinkedBoxSpawners(currentTeleportIndex);
    }

    void Update()
    {
        Idle.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        MoveToTarget();

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTarget();
        }
    }

    public void SetTargetForTeleportIndex(int teleportIndex)
    {
        if (teleportIndex >= 0 && teleportIndex < teleportPositions.Length)
        {
            Transform nextTeleportPosition = teleportPositions[teleportIndex];
            parent.transform.position = nextTeleportPosition.position;
            targetPosition = nextTeleportPosition.position;
        }
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
        HeartHitInstance = Instantiate(HeartHit, HeartHitSpawnPoint.position, HeartHitSpawnPoint.rotation);
        HeartHitInstance.Play();
        HeartHitInstance.gameObject.AddComponent<VFXAutoDestroy>();

        if (health <= 0 && !hasBeenDestroyed) 
        {
            hasBeenDestroyed = true;
            doorAnimator.Play("DoorOpening");
            

        }

        if (health <= 0) 
        {
            if (lastTeleportIndex == teleportPositions.Length - 1 || heartSpawner.currentPalier == heartSpawner.maxPalier)  
            {
                bossPatternManager.StopAllPatterns();
                InstantiateExplosion();
                Invoke("ShowYouWinUI", 0.5f); 
                return; 
            }

            Idle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            TeleportHeart();
            TeleportEyeRadius(transform.position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");
            Idle.start();

        }
    }
    void InstantiateExplosion()
    {
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    void ShowYouWinUI()
    {
        youWinPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");

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

            ActivateLinkedBoxSpawners(lastTeleportIndex);
            UpdateAccessibleTeleportPoints();
        }
    }

    public void TeleportEyeRadius(Vector3 newPosition)
    {
        eyeRadius.transform.position = newPosition;
        SetRandomTarget();
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

    private void ActivateLinkedBoxSpawners(int teleportIndex)
    {
        foreach (var pair in teleportPointBoxSpawnerPairs)
        {
            if (pair.teleportPointIndex == teleportIndex)
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

                if (pair.boxSpawnersNoHP != null)
                {
                    foreach (var boxSpawnerNoHP in pair.boxSpawnersNoHP)
                    {
                        if (boxSpawnerNoHP != null)
                        {
                            Debug.Log("Activating BoxSpawnerNoHP at teleport index: " + teleportIndex);
                            boxSpawnerNoHP.StartCoroutine(boxSpawnerNoHP.SpawnCube());
                        }
                        else
                        {
                            Debug.LogError("boxSpawnerNoHP is null in pair.boxSpawnersNoHP");
                        }
                    }
                }
                else
                {
                    Debug.LogError("pair.boxSpawnersNoHP is null");
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
