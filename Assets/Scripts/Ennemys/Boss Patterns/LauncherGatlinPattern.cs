using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GatlinLauncherPattern : MonoBehaviour
{
    [Header("Launcher Pattern Properties")]
    public float sphereRadius;
    public float sphereHeightOffset;
    public float sphereMovementDuration;
    public float launchInterval;
    public int cubesToLaunch;

    public HeartSpawner heartSpawner;


    public void SphereLauncherPattern()  // No longer takes numCubesToMove as an argument
    {
        if (heartSpawner == null)
        {
            Debug.LogError("HeartSpawner is not assigned.");
            return;
        }

        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        StartCoroutine(MoveCubesToSphere(generatedCubes, cubesToLaunch, () => {  // Use cubesToLaunch directly
            StartCoroutine(LaunchCubesOneByOne(generatedCubes));
            StartCoroutine(ResetPattern());
        }));
    }

    private IEnumerator ResetPattern()
    {
        yield return new WaitForSeconds(launchInterval * cubesToLaunch);
    }

    private IEnumerator MoveCubesToSphere(List<GameObject> cubes, int numCubes, System.Action onCompletion)
    {
        Vector3 sphereCenter = heartSpawner.transform.position + Vector3.up * sphereHeightOffset;

        cubes = cubes.Where(cube => cube != null).ToList();

        List<GameObject> cubesToRemove = new List<GameObject>(); // Liste pour stocker les cubes à retirer de la liste principale

        for (int i = 0; i < numCubes; i++)
        {
            if (i < cubes.Count)
            {
                GameObject cubeToMove = cubes[i];

                float polarAngle = Random.Range(0f, 180f);
                float azimuthAngle = Random.Range(0f, 360f);

                Vector3 spherePosition = CalculateSpherePosition(sphereCenter, polarAngle, azimuthAngle);

                StartCoroutine(MoveCubeToPosition(cubeToMove, spherePosition));

                cubesToRemove.Add(cubeToMove); // Ajoute le cube à la liste des cubes à retirer
                yield return new WaitForSeconds(launchInterval); // Attendre avant de déplacer le prochain cube
            }
        }

        // Retire les cubes sélectionnés pour le lancement de la liste principale
        foreach (var cubeToRemove in cubesToRemove)
        {
            cubes.Remove(cubeToRemove);
        }

        // Attendre que tous les cubes aient atteint la sphère avant de déclencher l'action suivante
        yield return new WaitForSeconds(sphereMovementDuration);

        onCompletion?.Invoke(); // Déclencher l'action suivante
    }

    private Vector3 CalculateSpherePosition(Vector3 center, float polarAngle, float azimuthAngle)
    {
        float radius = sphereRadius;

        float x = center.x + radius * Mathf.Sin(Mathf.Deg2Rad * polarAngle) * Mathf.Cos(Mathf.Deg2Rad * azimuthAngle);
        float y = center.y + radius * Mathf.Cos(Mathf.Deg2Rad * polarAngle);
        float z = center.z + radius * Mathf.Sin(Mathf.Deg2Rad * polarAngle) * Mathf.Sin(Mathf.Deg2Rad * azimuthAngle);

        return new Vector3(x, y, z);
    }

    private IEnumerator MoveCubeToPosition(GameObject cube, Vector3 targetPosition)
    {
        if (cube == null) yield break; // Vérifie si le cube a été détruit

        float elapsedTime = 0f;
        Vector3 initialPosition = cube.transform.position;

        while (elapsedTime < sphereMovementDuration)
        {
            if (cube == null) yield break; // Vérifie si le cube a été détruit

            cube.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / sphereMovementDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (cube == null) yield break; // Vérifie si le cube a été détruit

        cube.transform.position = targetPosition;
    }


    private IEnumerator LaunchCubesOneByOne(List<GameObject> cubes)
    {
        if (cubes == null || cubes.Count == 0)
        {
            Debug.LogError("List of cubes is null or empty.");
            yield break;
        }

        foreach (GameObject cube in cubes)
        {
            Rigidbody cubeRigidbody = cube.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            // Ajouter le script CubeDestroyer
            DestroyOnColPlayerGround destroyer = cube.AddComponent<DestroyOnColPlayerGround>();

            Vector3 playerPositionWithOffset = heartSpawner.playerPosition + Vector3.up * 3.15f;
            Vector3 launchDirection = (playerPositionWithOffset - cube.transform.position).normalized;

            cubeRigidbody.AddForce(launchDirection * heartSpawner.launchForce, ForceMode.Impulse);

            Destroy(cube, heartSpawner.cubeDestroyDelay);

            yield return new WaitForSeconds(launchInterval);
        }
    }

}
