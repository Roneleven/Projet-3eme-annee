using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HeartSpawner : MonoBehaviour
{
    [Header("Cubes Spawn Properties")]

    public GameObject cubePrefab;
    public GameObject transparentCubePrefab;
    public float spawnInterval;
    public float spawnRadius;
    public float gridSize;
    public float exclusionRadius;
    public float spawnCount;
    public GameObject spawnContainer;

    [Header("Palier Properties")]

    public HeartHealth heartHealth;
    public int previousPalier;
    public int currentPalier;
    public float temporarySpawnCount; //le spawncount pendant le changement de palier
    public float temporarySpawnInterval;//le spawninterval pendant le changement de palier
    public float timeTemporaryPalier; //la durée du changement de palier

    [Header("Timer/Reset Properties")]

    public float timer;
    public float defaultTimer;
    private bool timerActive = false;
    public TextMeshProUGUI timerText;
    public Image blackFade;
    public Animator anim;

    private bool isCooldownActive = false;

    private bool playerInPosition;
    private Vector3 playerPosition;

    [Header("Throw cube pattern Properties")]

    public int cubesGeneratedDuringPalier;
    public int offensivePatternThreshold;
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;

    private void Start()
    {
        StartCoroutine(SpawnCube());
    }

    private void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(PlayAnimationAndReload());
            }
            else
            {
                timerText.text = Mathf.Round(timer).ToString() + "s";
            }
        }
    }

    private IEnumerator SpawnCube()
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = Random.insideUnitSphere * spawnRadius;
                } while (spawnPosition.magnitude < exclusionRadius);

                spawnPosition /= gridSize;
                spawnPosition = new Vector3(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y), Mathf.Round(spawnPosition.z));
                spawnPosition *= gridSize;

                spawnPosition += transform.position;

                Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
                if (colliders.Length > 0)
                {
                    playerInPosition = false;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Player")
                        {
                            playerInPosition = true;
                            playerPosition = collider.transform.position;
                            break;
                        }
                        CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
                        if (cubeHealth != null)
                        {
                            if (cubeHealth.health < 26)
                            {
                                cubeHealth.health += 5;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    if (playerInPosition)
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                    }
                }
                else
                {
                    StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                }

                // Incrémente le compteur de cubes générés
                cubesGeneratedDuringPalier++;

                // Vérifie si le seuil est atteint pour déclencher le pattern offensif
                if (cubesGeneratedDuringPalier >= offensivePatternThreshold)
                {
                    StartCoroutine(TriggerOffensivePattern());
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator TriggerOffensivePattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        // Choisissez la moitié des cubes générés (ou un autre ratio selon vos besoins)
        int cubesToLaunch = Mathf.CeilToInt(generatedCubes.Count * (percentageToLaunch / 100f));

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            // Ajoute un Rigidbody au cube et active la gravité
            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerPosition = playerObject.transform.position;
            }
            // Calcule la direction de propulsion (vers le joueur)
            Vector3 launchDirection = (playerPosition - cubeToLaunch.transform.position).normalized;
            Debug.Log("Player Position: " + playerPosition);

            // Applique une force pour propulser le cube
            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            // Détruit le cube après un certain délai
            Destroy(cubeToLaunch, cubeDestroyDelay);
        }

        // Reset du compteur de cubes générés
        cubesGeneratedDuringPalier = 0;

        yield return null;
    }



    IEnumerator PlayAnimationAndReload()
    {
        anim.Play("FadeIn");
        yield return new WaitForSeconds(1);
        TimeOut();
    }

    private void TimeOut()
    {
        FindObjectOfType<SceneTransition>().ReloadScene();
    }

    private void UpgradeCubeIfNeeded(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gridSize / 2);
        foreach (Collider collider in colliders)
        {
            CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
            if (cubeHealth != null && cubeHealth.health < 26)
            {
                cubeHealth.health += 5;
                break;
            }
        }
    }

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Spawning", GetComponent<Transform>().position);
        yield return new WaitForSeconds(1);

        Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
        bool playerInPosition = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                playerInPosition = true;
                break;
            }
        }

        Destroy(transparentCube);

        if (!playerInPosition)
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Spawn", GetComponent<Transform>().position);
        }
        else
        {
            UpgradeCubeIfNeeded(spawnPosition);

            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            playerPosition /= gridSize;
            playerPosition = new Vector3(Mathf.Round(playerPosition.x), Mathf.Round(playerPosition.y), Mathf.Round(playerPosition.z));
            playerPosition *= gridSize;

            for (float x = playerPosition.x - 3; x <= playerPosition.x + 3; x += gridSize)
            {
                for (float y = playerPosition.y - 3; y <= playerPosition.y + 3; y += gridSize)
                {
                    for (float z = playerPosition.z - 3; z <= playerPosition.z + 3; z += gridSize)
                    {
                        Vector3 cubePosition = new Vector3(x, y, z);
                        if (Mathf.Abs(x - playerPosition.x) >= 3 || Mathf.Abs(y - playerPosition.y) >= 3 || Mathf.Abs(z - playerPosition.z) >= 3)
                        {
                            Instantiate(cubePrefab, cubePosition, Quaternion.identity, spawnContainer.transform);
                        }
                    }
                }
            }
        }
    }

    public void ChangePalierOnTeleport()
    {
        if (isCooldownActive) return;

        if (heartHealth != null)
        {
            timer = defaultTimer;

            if (currentPalier > previousPalier)
            {
                previousPalier = currentPalier;
            }

            timerActive = true;
            StartCoroutine(ResetPalier());
        }
    }

    private IEnumerator ResetPalier()
    {
        isCooldownActive = true;

        float originalSpawnRadius = spawnRadius;
        float originalSpawnCount = spawnCount;
        float originalSpawnInterval = spawnInterval;

        spawnCount = temporarySpawnCount;
        spawnInterval = temporarySpawnInterval;

        for (int palier = 1; palier <= currentPalier; palier++)
        {
            float temporarySpawnCountBeforeAdjust = spawnCount;
            AdjustPalierValues(palier);
            spawnCount = temporarySpawnCountBeforeAdjust;
            yield return new WaitForSeconds(timeTemporaryPalier);
        }

        // Restaurer les valeurs originales
        spawnRadius = originalSpawnRadius;
        spawnCount = originalSpawnCount;
        spawnInterval = originalSpawnInterval;

        // Passer au palier suivant
        currentPalier++;
        AdjustPalierValues(currentPalier);

        if (currentPalier >= 2 && !timerActive)
        {
            timerActive = true;
            Invoke("TimeOut", timer);
        }

        isCooldownActive = false;
    }

    private void AdjustPalierValues(int palier)
    {
        if (palier == 1)
        {
            spawnRadius = 4;
            spawnCount = 6;
        }
        else if (palier == 2)
        {
            spawnRadius = 8;
            spawnCount = 12;
        }
        else if (palier == 3)
        {
            spawnRadius = 12;
            spawnCount = 25;
        }
        else if (palier == 4)
        {
            spawnRadius = 16;
            spawnCount = 35;
        }
    }
}
