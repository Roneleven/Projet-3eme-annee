using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HeartSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius;
    public GameObject spawnContainer;
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount;
    public GameObject transparentCubePrefab;
    public HeartHealth heartHealth;
    public int previousPalier = 1;
    public int currentPalier = 1;
    public float temporarySpawnCount;
    public float timeTemporaryPalier;
    public float timer = 10;
    private bool timerActive = false;
    public TextMeshProUGUI timerText;
    public Image blackFade;
    public Animator anim;

    private bool isCooldownActive = false;

    private void Start()
    {
        StartCoroutine(SpawnCube());
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
                    bool playerInPosition = false;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Player")
                        {
                            playerInPosition = true;
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
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

void Update()
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

IEnumerator PlayAnimationAndReload()
{
    anim.Play("FadeIn");
     yield return new WaitForSeconds(1);
    TimeOut();
}

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity);
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
        }
        else
        {
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
        timer = 20; // Réinitialisez le timer ici

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

    spawnCount = temporarySpawnCount;

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

private void TimeOut()
{
    FindObjectOfType<SceneTransition>().ReloadScene();
}


    private void AdjustPalierValues(int palier)
    {


        if (palier == 1)
        {
            spawnRadius = 4; // Changer en fonction du palier 1
            spawnCount = 6; // Changer en fonction du palier 1
        }
        else if (palier == 2)
        {
            spawnRadius = 8; // Changer en fonction du palier 2
            spawnCount = 12; // Changer en fonction du palier 2
        }
        else if (palier == 3)
        {
            spawnRadius = 12; // Changer en fonction du palier 3
            spawnCount = 25; // Changer en fonction du palier 3
        }
        else if (palier == 4)
        {
            spawnRadius = 16; // Changer en fonction du palier 3
            spawnCount = 35; // Changer en fonction du palier 3
        }
    }
}
