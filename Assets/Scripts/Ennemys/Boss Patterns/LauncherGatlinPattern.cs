using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherGatlinPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner;
    public float sphereRadius = 5f;
    public float cubeMoveSpeed = 2f;
    public float launchInterval = 0.2f;
    public float launchDelay = 15f;

    private bool isLaunching = false;

    void Start()
    {
        StartCoroutine(LaunchPatternWithDelay());
    }

    IEnumerator LaunchPatternWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(launchDelay);

            if (!isLaunching)
            {
                isLaunching = true;
                SphereLauncherPattern();
            }
        }
    }

    public void SphereLauncherPattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));
        int cubesToLaunch = heartSpawner.cubesToLaunch;

        // Move cubes to form a sphere
        MoveCubesToSphere(generatedCubes);

        StartCoroutine(LaunchCubes(generatedCubes, cubesToLaunch));
    }

    void MoveCubesToSphere(List<GameObject> cubes)
    {
        int totalCubes = cubes.Count;
        float angleIncrement = 360f / totalCubes;

        for (int i = 0; i < totalCubes; i++)
        {
            float angle = i * angleIncrement;
            Vector3 targetPosition = transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * sphereRadius;

            StartCoroutine(MoveCubeToPosition(cubes[i], targetPosition));
        }
    }

    IEnumerator MoveCubeToPosition(GameObject cube, Vector3 targetPosition)
    {
        while (Vector3.Distance(cube.transform.position, targetPosition) > 0.1f)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, targetPosition, cubeMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator LaunchCubes(List<GameObject> cubes, int cubesToLaunch)
    {
        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = cubes[i];

            // Vérifier si le cubeToLaunch est null ou a été détruit
            if (cubeToLaunch != null)
            {
                Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
                cubeRigidbody.useGravity = true;

                Vector3 launchDirection = (heartSpawner.playerPosition - cubeToLaunch.transform.position).normalized;
                cubeRigidbody.AddForce(launchDirection * heartSpawner.launchForce, ForceMode.Impulse);

                // Modifier le tag du cube
                cubeToLaunch.tag = "LaunchedCube";

                Destroy(cubeToLaunch, heartSpawner.cubeDestroyDelay);
            }

            // Ajouter un petit délai avant de lancer le prochain cube
            yield return new WaitForSeconds(launchInterval);
        }

        // Réinitialiser le tag de tous les cubes après les avoir lancés
        foreach (var cube in cubes)
        {
            if (cube != null)
            {
                cube.tag = "LaunchedCube";
            }
        }

        heartSpawner.cubesGeneratedDuringPalier = 0;
        isLaunching = false; // Réinitialiser le drapeau une fois que tous les cubes ont été lancés
    }
}