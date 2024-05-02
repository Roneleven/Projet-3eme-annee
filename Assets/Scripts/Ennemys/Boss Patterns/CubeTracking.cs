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
