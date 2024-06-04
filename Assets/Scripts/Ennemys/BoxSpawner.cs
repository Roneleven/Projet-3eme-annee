using System.Collections;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius;
    public GameObject spawnContainer;
    public Vector3 spawnBoxSize = new Vector3(8f, 8f, 8f);
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount;
    public GameObject transparentCubePrefab;
    public int maxCubeCount = 50;
    public int cubeCount = 0;

    private bool isTriggerActivated = false;

    public void ActivateSpawner()
    {
        if (!isTriggerActivated)
        {
            isTriggerActivated = true;
            StartCoroutine(SpawnCube());
        }
    }

    public IEnumerator SpawnCube()
    {
        while (true)
        {
            if (!isTriggerActivated)
            {
                yield return null;
                continue;
            }

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
                                    cubeCount++;
                                }
                                else
                                {
                                    cubeCount++;
                                    cubeCount += Mathf.CeilToInt(cubeHealth.health / 5f) - 1;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                        cubeCount++;
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
            // Logic if player is in position (currently nothing is done here)
        }
        else
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
            cubeCount++;
        }

        cubeCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }
}
