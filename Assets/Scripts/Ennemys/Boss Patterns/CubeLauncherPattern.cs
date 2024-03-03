using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner;


    public void LauncherPattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));
        int cubesToLaunch = heartSpawner.cubesToLaunch;

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            Vector3 launchDirection = (heartSpawner.playerPosition - cubeToLaunch.transform.position).normalized;
            cubeRigidbody.AddForce(launchDirection * heartSpawner.launchForce, ForceMode.Impulse);

            Destroy(cubeToLaunch, heartSpawner.cubeDestroyDelay);
        }

        heartSpawner.cubesGeneratedDuringPalier = 0;   
    }
}
