using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 1f;
    public float spawnRadius = 4f;
    public float gridSize = 1f;
    public float exclusionRadius = 2f;
    public float spawnCount = 1; // Nombre de cubes à faire apparaître à chaque intervalle

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
                    foreach (Collider collider in colliders)
                    {
                        CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
                        if (cubeHealth != null)
                        {
                            cubeHealth.health += 5;
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