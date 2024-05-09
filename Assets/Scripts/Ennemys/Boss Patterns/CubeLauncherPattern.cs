using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class CubeLauncherPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner;

    public void Start()
    {
        heartSpawner = GetComponent<HeartSpawner>();
    }

    public void LauncherPattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));
        int cubesToLaunch = heartSpawner.cubesToLaunch;

         FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Patterns/CubeLauncher_Start");

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;
            DestroyOnColPlayerGround destroyer = cubeToLaunch.AddComponent<DestroyOnColPlayerGround>();
            CubeLauncherD damaging = cubeToLaunch.AddComponent<CubeLauncherD>();
            Vector3 playerPositionWithOffset = heartSpawner.playerPosition + Vector3.up * 3.15f;
            Vector3 launchDirection = (playerPositionWithOffset - cubeToLaunch.transform.position).normalized;
            cubeRigidbody.AddForce(launchDirection * heartSpawner.launchForce, ForceMode.Impulse);

            Destroy(cubeToLaunch, heartSpawner.cubeDestroyDelay);
        }
    }
}
