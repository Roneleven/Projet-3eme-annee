using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherPattern : MonoBehaviour
{
    public HeartSpawner heartSpawner; // R�f�rence � votre script principal HeartSpawner
    public int cubesGeneratedDuringPalier;
    public int offensivePatternThreshold; //multiple de la variable spawnCount
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;
    public Vector3 playerPosition;

    // Ajoutez d'autres variables n�cessaires ici

    private void Start()
    {
        // Initialisation si n�cessaire
    }

    private void Update()
    {
        // Ajoutez des mises � jour sp�cifiques au pattern de lancement de cube ici
    }

    public void TriggerOffensivePattern()
    {
        List<GameObject> generatedCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeartBlock"));

        // Choisissez la moiti� des cubes g�n�r�s (ou un autre ratio selon vos besoins)
        int cubesToLaunch = Mathf.CeilToInt(generatedCubes.Count * (percentageToLaunch / 100f));

        for (int i = 0; i < cubesToLaunch; i++)
        {
            GameObject cubeToLaunch = generatedCubes[i];

            // Ajoute un Rigidbody au cube et active la gravit�
            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            // Calcule la direction de propulsion (vers le joueur)
            Vector3 launchDirection = (playerPosition - cubeToLaunch.transform.position).normalized;

            // Applique une force pour propulser le cube
            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            // D�truit le cube apr�s un certain d�lai
            Destroy(cubeToLaunch, cubeDestroyDelay);
        }

        // R�initialisation du compteur de cubes g�n�r�s
        cubesGeneratedDuringPalier = 0;
    }

    // Ajoutez d'autres m�thodes n�cessaires ici
}
