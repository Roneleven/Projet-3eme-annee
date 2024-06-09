using System.Collections;
using UnityEngine;

public class BoxSpawnerActivateTrigger : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius;
    public GameObject spawnContainer;
    public Vector3 spawnBoxSize = new Vector3(8f, 8f, 8f);
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public int spawnCount;
    public GameObject transparentCubePrefab;
    public int maxCubeCount = 50;
    private int cubeCount = 0;

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
                int spawnedThisCycle = 0;
                for (int i = 0; i < spawnCount && cubeCount + spawnedThisCycle < maxCubeCount; i++)
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
                    bool positionOccupied = false;

                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("Player"))
                        {
                            positionOccupied = true;
                            break;
                        }

                        CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
                        if (cubeHealth != null)
                        {
                            if (cubeHealth.health < 6)
                            {
                                cubeHealth.health += 1;
                            }
                            else
                            {
                                cubeCount += Mathf.CeilToInt(cubeHealth.health / 5f) - 1;
                            }
                            positionOccupied = true;
                            break;
                        }
                    }

                    if (!positionOccupied)
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                        spawnedThisCycle++;
                    }
                }
                cubeCount += spawnedThisCycle;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        StartCoroutine(InterpolateFresnelPower(transparentCube));

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

        if (!playerInPosition)
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        }
        else
        {
            cubeCount--;
        }
    }

    private IEnumerator InterpolateFresnelPower(GameObject transparentCube)
    {
        Material transparentMaterial = transparentCube.GetComponent<Renderer>().material;
        float duration = 0.66667f;
        float elapsedTime = 0f;
        float initialFresnelPower = 4f;
        float targetFresnelPower = 10f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentFresnelPower = Mathf.Lerp(initialFresnelPower, targetFresnelPower, t);
            transparentMaterial.SetFloat("_FresnelPower", currentFresnelPower);

            yield return null;
        }

        transparentMaterial.SetFloat("_FresnelPower", targetFresnelPower);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }
}
