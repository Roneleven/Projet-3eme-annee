using UnityEngine;
using System.Collections;

public class BossPatternManager : MonoBehaviour
{
     public HeartSpawner heartSpawner; // Référence au script HeartSpawner
    private IEnumerator currentPatternCoroutine; // La coroutine du pattern actuel

    #region Wall Pattern

    [Header("WallPattern")]
    public GameObject wallPatternPrefab; // Modèle de pattern du mur
    public float wallSpawnInterval = 6f;

    private IEnumerator SpawnWallPattern()
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
    public int numberOfCubes; // Nombre de cubes à lancer
    public float radius; // Rayon autour du joueur pour le lancement des cubes
    public float journeyDuration = 40.0f; // Durée du mouvement des cubes
    public float launchInterval = 5f; // Interval entre chaque lancement

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

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (heartSpawner == null || wallPatternPrefab == null || player == null)
        {
            Debug.LogWarning("Missing references in BossPatternManager.");
            return;
        }

        // Abonnez-vous à l'événement de changement de palier du boss
        heartSpawner.OnPalierChange += OnPalierChange;
    }

    private void OnDestroy()
    {
        // Assurez-vous de vous désabonner à l'événement lorsque le script est détruit
        if (heartSpawner != null)
        {
            heartSpawner.OnPalierChange -= OnPalierChange;
        }
    }

    private void OnPalierChange(int newPalier)
{
    Debug.Log("New Palier: " + newPalier);

    // Arrêter la coroutine actuelle s'il y en a une
    if (currentPatternCoroutine != null)
    {
        StopCoroutine(currentPatternCoroutine);
        currentPatternCoroutine = null;
    }

    // Vérifiez si le boss a atteint le palier 2 et qu'aucune coroutine n'est déjà en cours
    if (newPalier == 2)
    {
        Debug.Log("Palier 2 reached, spawning wall pattern.");
        currentPatternCoroutine = SpawnWallPattern();
        StartCoroutine(currentPatternCoroutine);
    }
    // Si le boss atteint le palier 3, lancez le pattern des mines aériennes
    else if (newPalier == 3)
    {
        Debug.Log("Palier 3 reached, launching aerial mines pattern.");
        currentPatternCoroutine = LaunchAerialMinesPattern();
        StartCoroutine(currentPatternCoroutine);
    }
}

    
}