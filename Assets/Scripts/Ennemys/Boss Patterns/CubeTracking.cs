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


    public void LaunchHomingCubes()
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

            Rigidbody homingCubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            homingCubeRigidbody.useGravity = true;

            BoxCollider boxCollider = cubeToLaunch.GetComponent<BoxCollider>();

            // Si aucun BoxCollider n'est trouvé, en ajouter un
            if (boxCollider == null)
            {
                cubeToLaunch.AddComponent<BoxCollider>();
            }

            CubeHealth cubeHealthScript = cubeToLaunch.GetComponent<CubeHealth>();

            // Si le script CubeHealth n'est pas trouvé, l'ajouter
            if (cubeHealthScript == null)
            {
                if (cubeHealthScript == null)
                {
                    cubeHealthScript = cubeToLaunch.AddComponent<CubeHealth>();

                    // Créer un GameObject "Visual" comme enfant du cube
                    GameObject visualObject = new GameObject("Visual");
                    visualObject.transform.parent = cubeToLaunch.transform;

                    // Créer un GameObject "state_0" comme enfant de "Visual"
                    GameObject stateObject = new GameObject("state_0");
                    stateObject.transform.parent = visualObject.transform;

                    cubeHealthScript.health = 1;
                }
            }


            HomingCube homingCubeScript = cubeToLaunch.GetComponent<HomingCube>();
            if (homingCubeScript == null)
            {
                homingCubeScript = cubeToLaunch.AddComponent<HomingCube>();
            }

            homingCubeScript.SetTarget(targetTransform);
            homingCubeScript.SetDestroyDelay(destroyDelay);
            homingCubeScript.SetSpeed(homingCubeSpeed);
        }
    }
}
