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
    public float launchForce;
    public float launchInterval;
    public int cubesToLaunch;
    public Material newMaterial; // Nouveau matériau à appliquer

    private FMOD.Studio.EventInstance gatlin;

    public HeartSpawner heartSpawner;

    public void Start()
    {
        heartSpawner = GetComponent<HeartSpawner>();
        gatlin = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/Gatlin_Start");
    }

    void Update()
    {
        gatlin.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void SphereLauncherPattern()  // No longer takes numCubesToMove as an argument
    {
        gatlin.setParameterByName("Pattern", 0.0F);
        gatlin.start();

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
                GatlinD damaging = cubeToMove.AddComponent<GatlinD>();

                StartCoroutine(MoveCubeToPosition(cubeToMove, spherePosition));

                // Change the material of the children of the child
                ChangeMaterialOfChildChildren(cubeToMove, newMaterial);

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
        gatlin.setParameterByName("Pattern", 1.0F);
        if (cubes == null || cubes.Count == 0)
        {
            Debug.LogError("List of cubes is null or empty.");
            yield break;
        }

        int cubesLaunched = 0; // Variable pour compter les cubes lancés

        foreach (GameObject cube in cubes)
        {
            if (cubesLaunched >= cubesToLaunch)
            {
                // Si le nombre de cubes lancés atteint cubesToLaunch, sortir de la boucle
                yield break;
            }

            if (cube == null)
            {
                // Si le cube a été détruit, passer au suivant
                continue;
            }

            // Changer le tag de "HeartBlock" à "Block"
            if (cube.tag == "HeartBlock")
            {
                cube.tag = "Block";
            }

            Rigidbody cubeRigidbody = cube.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            // Ajouter le script CubeDestroyer
            DestroyOnColPlayerGround destroyer = cube.AddComponent<DestroyOnColPlayerGround>();

            Vector3 playerPositionWithOffset = heartSpawner.playerPosition + Vector3.up * 3.15f;
            Vector3 launchDirection = (playerPositionWithOffset - cube.transform.position).normalized;

            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Patterns/Gatlin_Shoot");

            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Destroy(cube, heartSpawner.cubeDestroyDelay);

            cubesLaunched++; // Incrémenter le nombre de cubes lancés

            yield return new WaitForSeconds(launchInterval);
        }
    }

    private void ChangeMaterialOfChildChildren(GameObject parentObject, Material newMaterial)
    {
        if (parentObject.transform.childCount > 0)
        {
            Transform child = parentObject.transform.GetChild(0);
            foreach (Transform grandChild in child)
            {
                Renderer renderer = grandChild.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = newMaterial;
                }
            }
        }
    }
}