using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTracking : MonoBehaviour
{
    public HeartSpawner heartSpawner;
    public int numberOfCubesToLaunch;
    public float homingCubeSpeed;
    public float destroyDelay;
    public float cubeTimer;
    public Transform targetTransform;

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

            // Ajoute un Rigidbody au cube à tête chercheuse
            Rigidbody homingCubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            homingCubeRigidbody.useGravity = true;


            // Ajoute ou récupère le script HomingCube
            HomingCube homingCubeScript = cubeToLaunch.GetComponent<HomingCube>();
            if (homingCubeScript == null)
            {
                // Ajoute le script HomingCube s'il n'est pas déjà présent
                homingCubeScript = cubeToLaunch.AddComponent<HomingCube>();
            }

            // Configure le cube à tête chercheuse
            homingCubeScript.SetTarget(targetTransform);
            homingCubeScript.SetDestroyDelay(5f);
            homingCubeScript.SetSpeed(homingCubeSpeed);
        }
    }
}
