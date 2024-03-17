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

        StartCoroutine(LaunchWithDelay(generatedCubes));
    }

    private IEnumerator LaunchWithDelay(List<GameObject> cubes)
    {
        foreach (GameObject cube in cubes)
        {
            if (cube == null)
            {
                Debug.LogWarning("Cube to launch is null.");
                continue;
            }

            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                // Change la couleur du cube en noir
                cubeRenderer.material.color = Color.black;
            }

            // Attend 2 secondes avant de lancer le cube
            yield return new WaitForSeconds(2f);

            // Ensuite, lance le cube comme avant
            Rigidbody homingCubeRigidbody = cube.AddComponent<Rigidbody>();
            homingCubeRigidbody.useGravity = true;

            HomingCube homingCubeScript = cube.GetComponent<HomingCube>();
            if (homingCubeScript == null)
            {
                homingCubeScript = cube.AddComponent<HomingCube>();
            }

            homingCubeScript.SetTarget(targetTransform);
            homingCubeScript.SetDestroyDelay(destroyDelay);
            homingCubeScript.SetSpeed(homingCubeSpeed);
        }
    }

}
