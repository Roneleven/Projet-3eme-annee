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
    public int maxCubeCount = 50;

    // Variable pour suivre le nombre actuel de blocs r�els
    public int cubeCount = 0;

    private void Start()
    {
        //StartCoroutine(SpawnCube());
    }

	public IEnumerator SpawnCube()
    {
        while (true)
        {
           // while (pause) yield return new WaitForEndOfFrame();

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
                                if (cubeHealth.health < 6)
                                {
                                    cubeHealth.health += 1;
                                    cubeCount++; // Incr�mente le nombre de blocs r�els
                                }
                                else
                                {
                                    // Cube am�lior�, comptez-le comme un cube suppl�mentaire
                                    cubeCount++; // Incr�mente le nombre de blocs r�els

                                    // Ajoutez ici la logique d'augmentation du cubeCount en fonction de l'am�lioration du cube
                                    cubeCount += Mathf.CeilToInt(cubeHealth.health / 5f) - 1;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                        cubeCount++; // Incr�mente le nombre de blocs r�els
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
