using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTracking : MonoBehaviour
{
    public HeartSpawner heartSpawner;
    public GameObject homingCubePrefab;
    public int numberOfCubesToLaunch;
    public float homingCubeSpeed;
    public float destroyDelay;
    public float cubeTimer;

    private void Start()
    {
        StartCoroutine(StartHomingCubePattern());
    }

    IEnumerator StartHomingCubePattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(cubeTimer);
            LaunchHomingCubes();
        }
    }

    void LaunchHomingCubes()
    {
        if (heartSpawner == null)
        {
            Debug.LogError("heartSpawner is not assigned.");
            return;
        }

        if (homingCubePrefab == null)
        {
            Debug.LogError("homingCubePrefab is not assigned.");
            return;
        }

        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        if (generatedCubes.Count == 0)
        {
            Debug.LogWarning("No HeartBlock cubes found.");
            return;
        }

        List<GameObject> cubesToLaunch = new List<GameObject>();
        int numberOfCubes = Mathf.Min(numberOfCubesToLaunch, generatedCubes.Count);

        for (int i = 0; i < numberOfCubes; i++)
        {
            int randomIndex = Random.Range(0, generatedCubes.Count);
            cubesToLaunch.Add(generatedCubes[randomIndex]);
            generatedCubes.RemoveAt(randomIndex);
        }

        foreach (GameObject cubeToLaunch in cubesToLaunch)
        {
            if (cubeToLaunch == null)
            {
                Debug.LogWarning("Cube to launch is null.");
                continue;
            }

            // Instancie un cube à tête chercheuse
            GameObject homingCube = Instantiate(homingCubePrefab, cubeToLaunch.transform.position, Quaternion.identity);

            // Attache dynamiquement le script HomingCube
            HomingCube homingCubeScript = homingCube.AddComponent<HomingCube>();
            Rigidbody homingCubeRigidbody = homingCube.AddComponent<Rigidbody>();
            homingCubeRigidbody.useGravity = true;


           
            // Configure le cube à tête chercheuse
            homingCubeScript.SetTarget(heartSpawner.playerPosition);
            homingCubeScript.SetDestroyDelay(destroyDelay);
            homingCubeScript.SetSpeed(homingCubeSpeed);
        }
    }
}
