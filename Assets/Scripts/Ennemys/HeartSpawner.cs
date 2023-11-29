using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius = 4f;
    public GameObject spawnContainer;
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount = 1; // Nombre de cubes à faire apparaître à chaque intervalle
    public GameObject transparentCubePrefab;
    public HeartHealth heartHealth;

    private void Start()
    {
        StartCoroutine(SpawnCube());
        StartCoroutine(ChangeSpawnRadiusAndCount());
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
            }
            while (spawnPosition.magnitude < exclusionRadius);

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
                    StartCoroutine(SpawnTransparentAndRealCube(spawnPosition)); // Appeler la nouvelle coroutine
                }
            }
else
{
    StartCoroutine(SpawnTransparentAndRealCube(spawnPosition)); // Appeler la nouvelle coroutine
}
        }

        yield return new WaitForSeconds(spawnInterval);
    }
}

private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition) // Nouvelle coroutine
{
    GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity); // Instanciez le cube semi-transparent
    yield return new WaitForSeconds(1); // Attendez une seconde

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

    Destroy(transparentCube); // Détruisez le cube semi-transparent

    if (!playerInPosition)
    {
        Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform); // Instanciez le cube réel
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
                        Instantiate(cubePrefab, cubePosition, Quaternion.identity, spawnContainer.transform); // Instanciez le cube réel
                    }
                }
            }
        }
    }
}

    private IEnumerator ChangeSpawnRadiusAndCount()
    {
        yield return new WaitForSeconds(30);
        spawnRadius = 8;
        spawnCount = 4; // Faire apparaître 2 cubes à chaque intervalle après 30 secondes

        yield return new WaitForSeconds(60);
        spawnRadius = 12;
        spawnCount = 8;

        yield return new WaitForSeconds(90);
        spawnRadius = 16;
        spawnCount = 16;

        yield return new WaitForSeconds(120);
        spawnRadius = 20;
        spawnCount = 32;

        yield return new WaitForSeconds(150);
        spawnRadius = 24;
        spawnCount = 48;

        yield return new WaitForSeconds(180);
        spawnRadius = 28;
        spawnCount = 64;
    }

}