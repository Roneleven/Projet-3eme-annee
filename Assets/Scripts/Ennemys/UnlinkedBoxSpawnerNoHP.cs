using System.Collections;
using UnityEngine;

public class UnlinkedBoxSpawnerNoHP : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius;
    public GameObject spawnContainer;
    public Vector3 spawnBoxSize = new Vector3(8f, 8f, 8f); // Taille de la boîte de spawn
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount;
    public GameObject transparentCubePrefab;
    public int maxCubeCount = 50;

    // Variable pour suivre le nombre actuel de blocs réels
    public int cubeCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnCube());
    }

    public IEnumerator SpawnCube()
    {
        while (true)
        {
            if (cubeCount < maxCubeCount)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Vector3 spawnPosition;
                    do
                    {
                        spawnPosition = new Vector3(
                            Random.Range(-spawnBoxSize.x / 2, spawnBoxSize.x / 2),
                            Random.Range(-spawnBoxSize.y / 2, spawnBoxSize.y / 2),
                            Random.Range(-spawnBoxSize.z / 2, spawnBoxSize.z / 2)
                        );
                    } while (spawnPosition.magnitude < exclusionRadius);

                    spawnPosition /= gridSize;
                    spawnPosition = new Vector3(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y), Mathf.Round(spawnPosition.z));
                    spawnPosition *= gridSize;

                    spawnPosition += transform.position;

                    Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
                    if (colliders.Length > 0)
                    {
                        foreach (Collider collider in colliders)
                        {
                            bool playerInPosition = collider.gameObject.CompareTag("Player");
                            if (playerInPosition)
                            {
                                StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                                break;
                            }

                            CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
                            if (cubeHealth != null)
                            {
                                // Logique pour ajouter de la vie au cube
                                break;
                            }
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                        cubeCount++; // Incrémente le nombre de blocs réels
                    }
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        yield return new WaitForSeconds(spawnInterval);

        Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
        bool playerInPosition = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                playerInPosition = true;
                break;
            }
        }

        Destroy(transparentCube);

        if (playerInPosition)
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
                            cubeCount++; // Incrémente le nombre de blocs réels
                        }
                    }
                }
            }
        }
        else
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
            cubeCount++; // Incrémente le nombre de blocs réels
        }

        // Décrémente le nombre de blocs réels lorsque la coroutine est terminée (quand le bloc transparent est détruit)
        cubeCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }
}
