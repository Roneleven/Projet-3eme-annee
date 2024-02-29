using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner;


    public void LauncherPattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        // Choisissez la moitié des cubes générés (ou un autre ratio selon vos besoins)
        //int cubesToLaunch = Mathf.CeilToInt(generatedCubes.Count * (heartSpawner.percentageToLaunch / 100f));
        int cubesToLaunch = heartSpawner.cubesToLaunch;

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            // Ajoute un Rigidbody au cube et active la gravité
            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            // Calcule la direction de propulsion (vers le joueur)
            Vector3 launchDirection = (heartSpawner.playerPosition - cubeToLaunch.transform.position).normalized;
            Debug.Log("Player Position: " + heartSpawner.playerPosition);

            // Applique une force pour propulser le cube
            cubeRigidbody.AddForce(launchDirection * heartSpawner.launchForce, ForceMode.Impulse);

            // Détruit le cube après un certain délai
            Destroy(cubeToLaunch, heartSpawner.cubeDestroyDelay);
        }

        // Reset du compteur de cubes générés
        heartSpawner.cubesGeneratedDuringPalier = 0;
    }
}
