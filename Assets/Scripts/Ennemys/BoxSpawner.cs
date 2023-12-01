using System.Collections;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius;
    public GameObject spawnContainer;
    public Vector3 spawnBoxSize = new Vector3(8f, 8f, 8f); // Taille de la bo�te de spawn
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount;
    public GameObject transparentCubePrefab;

    // Nouvelle variable pour d�finir le nombre maximal de blocs dans l'inspecteur Unity
    public int maxCubeCount;

    // Variable pour suivre le nombre actuel de blocs r�els
    public int cubeCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnCube());
    }

    private IEnumerator SpawnCube()
    {
        while (true)
        {
            if (cubeCount < maxCubeCount)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Vector3 spawnPosition;

                    // Utilisez un nombre limit� de tentatives pour �viter une boucle infinie
                    int maxAttempts = 10;
                    int attemptCount = 0;

                    do
                    {
                        spawnPosition = new Vector3(
                            Random.Range(-spawnBoxSize.x / 2, spawnBoxSize.x / 2),
                            Random.Range(-spawnBoxSize.y / 2, spawnBoxSize.y / 2),
                            Random.Range(-spawnBoxSize.z / 2, spawnBoxSize.z / 2)
                        );

                        spawnPosition /= gridSize;
                        spawnPosition = new Vector3(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y), Mathf.Round(spawnPosition.z));
                        spawnPosition *= gridSize;

                        spawnPosition += transform.position;

                        attemptCount++;
                    } while (IsSpawnPositionColliding(spawnPosition) && attemptCount < maxAttempts);

                    // V�rifiez � nouveau si la position est en collision avec le sol
                    if (!IsSpawnPositionColliding(spawnPosition))
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                        cubeCount++; // Incr�mente le nombre de blocs r�els
                    }
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Fonction pour v�rifier si la position de spawn est en collision avec le sol
    private bool IsSpawnPositionColliding(Vector3 position)
    {
        // Ajustez la hauteur de la ligne en fonction de votre cube et de la pr�cision n�cessaire
        float lineHeight = gridSize;

        Vector3 start = position + Vector3.up * lineHeight;
        Vector3 end = position - Vector3.up * lineHeight;

        if (Physics.Linecast(start, end))
        {
            // Ajoutez ici toutes les couches que vous souhaitez �viter (par exemple, Ground)
            if (Physics.Linecast(start, end, 1 << LayerMask.NameToLayer("Ground")))
            {
                return true;
            }
        }

        return false;
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
                            cubeCount++; // Incr�mente le nombre de blocs r�els
                        }
                    }
                }
            }
        }
        else
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
            cubeCount++; // Incr�mente le nombre de blocs r�els
        }

        // D�cr�mente le nombre de blocs r�els lorsque la coroutine est termin�e (quand le bloc transparent est d�truit)
        cubeCount--;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }
}
