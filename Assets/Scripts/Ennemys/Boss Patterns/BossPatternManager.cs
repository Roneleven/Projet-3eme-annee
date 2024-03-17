using UnityEngine;
using System.Collections;

public class BossPatternManager : MonoBehaviour
{
     public HeartSpawner heartSpawner;
    private IEnumerator currentPatternCoroutine; 

    #region Wall Pattern

    [Header("WallPattern")]
    public GameObject wallPatternPrefab; // Modèle de pattern du mur
    public float wallSpawnInterval = 6f;

    private IEnumerator LaunchWallPattern()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(wallSpawnInterval);

        while (true)
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 heartPosition = transform.position;
            
            Vector3 middlePoint = (playerPosition + heartPosition) / 2f;
            Vector3 spawnPosition = middlePoint + (heartPosition - middlePoint).normalized * 10f;
            spawnPosition.y = playerPosition.y;

            GameObject wallPatternInstance = Instantiate(wallPatternPrefab, spawnPosition, Quaternion.identity);

            Vector3 directionToPlayer = (playerPosition - spawnPosition).normalized;
            Vector3 xzOnlyDirectionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(xzOnlyDirectionToPlayer, Vector3.up);
            wallPatternInstance.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y + 180f, 0);

            yield return waitInterval;
        }
    }
    #endregion



     #region Aerial Mines Pattern
     [Header("AerialMines")]
    public GameObject cubePrefab; // Modèle de cube pour le pattern des mines aériennes
    public int numberOfCubes;
    public float radius;
    public float journeyDuration = 40.0f; // Durée du mouvement des cubes
    public float launchInterval = 5f;

    private IEnumerator LaunchAerialMinesPattern()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(launchInterval);

        while (true)
        {
            yield return waitInterval;

            LaunchAerialPattern();
        }
    }

    private void LaunchAerialPattern()
    {
        if (player != null)
        {
            Transform playerTransform = player.transform;

            for (int i = 0; i < numberOfCubes; i++)
            {
                Vector3 randomPosition = playerTransform.position + Random.insideUnitSphere * radius;
                GameObject newCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
                StartCoroutine(MoveToRandomPosition(newCube.transform, randomPosition));
            }
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure you have a GameObject tagged 'Player' in your scene.");
        }
    }

    private IEnumerator MoveToRandomPosition(Transform cubeTransform, Vector3 targetPosition)
    {
        float startTime = Time.time;
        Vector3 startPosition = cubeTransform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);

        while (cubeTransform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * journeyDuration;
            float fractionOfJourney = distanceCovered / journeyLength;
            cubeTransform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }
    }
    #endregion



    #region Explosive Pillar Pattern
    [Header("ExplosivePillar")]
    public GameObject emptyPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 7f;
    private GameObject playerObject;
    private LayerMask groundLayerMask;

    private IEnumerator LaunchExplosivePillarPattern()
{
    WaitForSeconds waitInterval = new WaitForSeconds(spawnInterval);

    while (true)
    {
        Vector2 randomPoint2D = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPoint = new Vector3(randomPoint2D.x, 0f, randomPoint2D.y) + playerObject.transform.position;
        randomPoint.z += Random.Range(-spawnRadius, spawnRadius);

        Collider[] colliders = Physics.OverlapSphere(randomPoint, 20f, groundLayerMask);
        if (colliders.Length == 0)
        {
            yield return null;
        }

        if (!Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            yield return null;
        }

        GameObject emptyObject = Instantiate(emptyPrefab, hit.point, Quaternion.identity);
        Vector3 direction = playerObject.transform.position - hit.point;
        direction.y = 0f;
        emptyObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        yield return waitInterval;
    }
}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
    #endregion



    private GameObject player;

    private void Start()
{
    player = GameObject.FindGameObjectWithTag("Player");

    if (heartSpawner == null || wallPatternPrefab == null || player == null)
    {
        Debug.LogWarning("Missing references in BossPatternManager.");
        return;
    }

    heartSpawner.OnPalierChange += OnPalierChange;

    playerObject = GameObject.FindGameObjectWithTag("Player");
    if (playerObject == null)
    {
        Debug.LogError("Player not found with tag: Player");
        return;
    }

    groundLayerMask = LayerMask.GetMask("Ground");
}

    private void OnDestroy()
    {
        if (heartSpawner != null)
        {
            heartSpawner.OnPalierChange -= OnPalierChange;
        }
    }

    private void OnPalierChange(int newPalier)
    {
        Debug.Log("New Palier: " + newPalier);
        
        if (currentPatternCoroutine != null)
        {
            StopCoroutine(currentPatternCoroutine);
            currentPatternCoroutine = null;
        }

        if (newPalier == 1)
        {
            return;
        }

        else if (newPalier == 2)
        {
            currentPatternCoroutine = LaunchWallPattern();
            StartCoroutine(currentPatternCoroutine);
        }
        else if (newPalier == 3)
        {
            currentPatternCoroutine = LaunchAerialMinesPattern();
            StartCoroutine(currentPatternCoroutine);
        }
        else if (newPalier == 4)
        {
            currentPatternCoroutine = LaunchExplosivePillarPattern();
            StartCoroutine(currentPatternCoroutine);
        }
    }

}