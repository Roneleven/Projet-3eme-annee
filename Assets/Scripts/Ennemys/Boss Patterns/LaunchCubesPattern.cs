using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner; // Référence à votre script principal HeartSpawner
    public int cubesGeneratedDuringPalier;
    public int offensivePatternThreshold; //multiple de la variable spawnCount
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;
    public Vector3 playerPosition;

    // Ajoutez d'autres variables nécessaires ici

    private void Start()
    {
        // Initialisation si nécessaire
    }

    private void Update()
    {
        // Ajoutez des mises à jour spécifiques au pattern de lancement de cube ici
    }

    public void TriggerOffensivePattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        // Choisissez la moitié des cubes générés (ou un autre ratio selon vos besoins)
        int cubesToLaunch = Mathf.CeilToInt(generatedCubes.Count * (percentageToLaunch / 100f));

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            // Ajoute un Rigidbody au cube et active la gravité
            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            // Calcule la direction de propulsion (vers le joueur)
            Vector3 launchDirection = (playerPosition - cubeToLaunch.transform.position).normalized;

            // Applique une force pour propulser le cube
            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            // Détruit le cube après un certain délai
            Destroy(cubeToLaunch, cubeDestroyDelay);
        }

        // Réinitialisation du compteur de cubes générés
        cubesGeneratedDuringPalier = 0;
    }

    // Ajoutez d'autres méthodes nécessaires ici
}
