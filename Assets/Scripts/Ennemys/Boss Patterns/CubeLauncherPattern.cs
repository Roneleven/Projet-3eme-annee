using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LaunchCubesPattern : MonoBehaviour
{
    public int cubesGeneratedDuringPalier;
    public int offensivePatternThreshold;
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;

    private Vector3 playerPosition;

    // Appelé pour déclencher le pattern de lancement de cubes
    public void DoCubeLaunchPattern(Vector3 playerPos)
    {
        playerPosition = playerPos;

        StartCoroutine(TriggerOffensivePattern());
    }

    private IEnumerator TriggerOffensivePattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        int cubesToLaunch = Mathf.CeilToInt(generatedCubes.Count * (percentageToLaunch / 100f));

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];
            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            Vector3 launchDirection = (playerPosition - cubeToLaunch.transform.position).normalized;

            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Destroy(cubeToLaunch, cubeDestroyDelay);
        }

        cubesGeneratedDuringPalier = 0;

        yield return null;
    }
}
