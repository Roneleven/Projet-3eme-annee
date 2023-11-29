using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public Vector3 spawnBoxSize = new Vector3(8f, 8f, 8f); // Taille de la boîte de spawn
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount = 1; // Nombre de cubes à faire apparaître à chaque intervalle

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
                    spawnPosition = new Vector3(
                        Random.Range(-spawnBoxSize.x / 2, spawnBoxSize.x / 2),
                        Random.Range(-spawnBoxSize.y / 2, spawnBoxSize.y / 2),
                        Random.Range(-spawnBoxSize.z / 2, spawnBoxSize.z / 2)
                    );
                }
                while (spawnPosition.magnitude < exclusionRadius);

                spawnPosition /= gridSize;
                spawnPosition = new Vector3(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y), Mathf.Round(spawnPosition.z));
                spawnPosition *= gridSize;

                spawnPosition += transform.position;

                Collider[] colliders = Physics.OverlapBox(spawnPosition, new Vector3(gridSize / 2, gridSize / 2, gridSize / 2));
                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
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
                }
                else
                {
                    Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }
}