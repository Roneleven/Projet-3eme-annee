using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HeartSpawner : MonoBehaviour
{
    [Header("Cubes Spawn Properties")]
    public GameObject cubePrefab;
    public GameObject transparentCubePrefab;
    public float spawnInterval;
    public float spawnRadius;
    public float gridSize;
    public float exclusionRadius;
    public float spawnCount;
    public GameObject spawnContainer;

    [Header("Palier Properties")]
    public HeartHealth heartHealth;
    public int previousPalier;
    public int currentPalier;
    public float temporarySpawnCount; //le spawncount pendant le changement de palier
    public float temporarySpawnInterval;//le spawninterval pendant le changement de palier
    public float timeTemporaryPalier; //la durée du changement de palier

    private FMOD.Studio.EventInstance BreakingHeart;

    [Header("Timer/Reset Properties")]
    public float timer;
    public float defaultTimer;
    private bool timerActive = false;
    public TextMeshProUGUI timerText;
    public Image blackFade;
    public Animator anim;

    private bool isCooldownActive = false;

    private bool playerInPosition;
    public Vector3 playerPosition;

    [Header("Throw cube pattern Properties")]
    public int cubesGeneratedDuringPalier;
    public int offensivePatternThreshold; //multiple de la variable spawnCount
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;

    [Header("Wall pattern Properties")]
    public float wallSpawnInterval = 10f;
    public GameObject wallPrefab;
    public float wallDistance = 10f;
    public float wallWidth = 3f;
    public float wallHeight = 3f;
    public float wallSpeed = 10f;
    public MouseLook mouseLookScript;

    private WallPattern wallPattern;
    public CubeLauncherPattern cubeLauncherPattern;


    private void Start()
    {
        cubeLauncherPattern = new CubeLauncherPattern();
        cubeLauncherPattern.heartSpawner = this;
        wallPattern = new WallPattern();
        StartCoroutine(SpawnCube());
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Idle", GetComponent<Transform>().position);
        BreakingHeart = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/BreakingTheHeart");
        BreakingHeart.start();

        //StartCoroutine(SpawnWallPattern());
    }

    /*private IEnumerator SpawnWallPattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(wallSpawnInterval);

            // Crée un mur dans la direction du joueur
            SpawnWall();
        }
    }

    private void SpawnWall()
    {
        Quaternion wallRotation = Quaternion.Euler(0f, mouseLookScript.transform.eulerAngles.y, 0f);
        Vector3 wallPosition = mouseLookScript.transform.position +
                               mouseLookScript.transform.forward * wallDistance;

        // Ajuste la position Y pour correspondre à la hauteur du joueur
        wallPosition.y = mouseLookScript.transform.position.y;

        GameObject wall = Instantiate(wallPrefab, wallPosition, wallRotation);
        wall.transform.localScale = new Vector3(wallWidth, wallHeight, 1f);

        StartCoroutine(MoveWall(wall.transform));
    }


    private IEnumerator MoveWall(Transform wallTransform)
    {
        // Durée totale du mouvement du mur
        float moveDuration = wallSpawnInterval;

        // Temps écoulé
        float elapsedTime = 0f;

        // Position initiale du mur
        Vector3 initialPosition = wallTransform.position;

        // Position cible du mur (avancer dans la direction locale Z)
        Vector3 targetPosition = initialPosition - wallTransform.forward * wallDistance;

        while (elapsedTime < moveDuration)
        {
            // Calcule la position intermédiaire en fonction du temps écoulé
            float t = elapsedTime / moveDuration;
            wallTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            // Met à jour le temps écoulé
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Assurez-vous que le mur est à la position finale exacte
        wallTransform.position = targetPosition;
    }
    */
    private void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(PlayAnimationAndReload());
            }
            else
            {
                timerText.text = Mathf.Round(timer).ToString() + "s";
            }
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerPosition = playerObject.transform.position;
        }

        BreakingHeart.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        //wallPattern.UpdateWallPattern();

        // Condition d'activation pour le pattern offensif
        if (cubesGeneratedDuringPalier >= offensivePatternThreshold)
        {
            cubeLauncherPattern.TriggerOffensivePattern();
        }
    }

    private IEnumerator SpawnCube()
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = Random.insideUnitSphere * spawnRadius;
                } while (spawnPosition.magnitude < exclusionRadius);

                spawnPosition /= gridSize;
                spawnPosition = new Vector3(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y), Mathf.Round(spawnPosition.z));
                spawnPosition *= gridSize;

                spawnPosition += transform.position;

                Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
                if (colliders.Length > 0)
                {
                    playerInPosition = false;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Player")
                        {
                            playerInPosition = true;
                            playerPosition = collider.transform.position;
                            break;
                        }
                        CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
                        if (cubeHealth != null)
                        {
                            if (cubeHealth.health < 6)
                            {
                                cubeHealth.health += 1;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    if (playerInPosition)
                    {
                        StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                    }
                }
                else
                {
                    StartCoroutine(SpawnTransparentAndRealCube(spawnPosition));
                }

                // Incrémente le compteur de cubes générés
                cubesGeneratedDuringPalier++;

                // Vérifie si le seuil est atteint pour déclencher le pattern offensif
               /* if (cubesGeneratedDuringPalier >= offensivePatternThreshold)
                {
                    cubeLauncherPattern.TriggerOffensivePattern();
                }*/
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /*private IEnumerator TriggerOffensivePattern()
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
            Debug.Log("Player Position: " + playerPosition);

            // Applique une force pour propulser le cube
            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            // Détruit le cube après un certain délai
            Destroy(cubeToLaunch, cubeDestroyDelay);
        }

        // Reset du compteur de cubes générés
        cubesGeneratedDuringPalier = 0;

        yield return null;
    }
    */


    IEnumerator PlayAnimationAndReload()
    {
        anim.Play("FadeIn");
        yield return new WaitForSeconds(1);
        TimeOut();
    }

    private void TimeOut()
    {
        BreakingHeart.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FindObjectOfType<SceneTransition>().ReloadScene();
    }

    private void UpgradeCubeIfNeeded(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gridSize / 2);
        foreach (Collider collider in colliders)
        {
            CubeHealth cubeHealth = collider.gameObject.GetComponent<CubeHealth>();
            if (cubeHealth != null && cubeHealth.health < 6)
            {
                cubeHealth.health += 1;
                break;
            }
        }
    }

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Spawning", GetComponent<Transform>().position);
        yield return new WaitForSeconds(spawnInterval);

        Collider[] colliders = Physics.OverlapSphere(spawnPosition, gridSize / 2);
        bool playerInPosition = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                playerInPosition = true;
                break;
            }
        }

        Destroy(transparentCube);

        if (!playerInPosition)
        {
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        }
        else
        {
            UpgradeCubeIfNeeded(spawnPosition);

            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            playerPosition /= gridSize;
            playerPosition = new Vector3(Mathf.Round(playerPosition.x), Mathf.Round(playerPosition.y), Mathf.Round(playerPosition.z));
            playerPosition *= gridSize;

            for (float x = playerPosition.x - 3; x <= playerPosition.x + 3; x += gridSize)
            {
                for (float y = playerPosition.y - 3; y <= playerPosition.y + 3; y += gridSize)
                {
                    for (float z = playerPosition.z - 3; z <= playerPosition.z + 3; z += gridSize)
                    {
                        Vector3 cubePosition = new Vector3(x, y, z);
                        if (Mathf.Abs(x - playerPosition.x) >= 3 || Mathf.Abs(y - playerPosition.y) >= 3 || Mathf.Abs(z - playerPosition.z) >= 3)
                        {
                            Instantiate(cubePrefab, cubePosition, Quaternion.identity, spawnContainer.transform);
                            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Cage/Traped");
                        }
                    }
                }
            }
        }
    }

    public void ChangePalierOnTeleport()
    {
        if (isCooldownActive) return;

        if (heartHealth != null)
        {
            timer = defaultTimer;

            if (currentPalier > previousPalier)
            {
                previousPalier = currentPalier;
            }

            float newLevelUpValue = (currentPalier + 1) * 1.0f;
            BreakingHeart.setParameterByName("LevelUp", newLevelUpValue);

            timerActive = true;
            StartCoroutine(ResetPalier());
        }
    }

    private IEnumerator ResetPalier()
    {
        isCooldownActive = true;

        float originalSpawnRadius = spawnRadius;
        float originalSpawnCount = spawnCount;
        float originalSpawnInterval = spawnInterval;

        spawnCount = temporarySpawnCount;
        spawnInterval = temporarySpawnInterval;

        for (int palier = 1; palier <= currentPalier; palier++)
        {
            float temporarySpawnCountBeforeAdjust = spawnCount;
            AdjustPalierValues(palier);
            spawnCount = temporarySpawnCountBeforeAdjust;
            yield return new WaitForSeconds(timeTemporaryPalier);
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Annonce/CoconForme");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Idle", GetComponent<Transform>().position);
        // Restaurer les valeurs originales
        spawnRadius = originalSpawnRadius;
        spawnCount = originalSpawnCount;
        spawnInterval = originalSpawnInterval;

        // Passer au palier suivant
        currentPalier++;
        AdjustPalierValues(currentPalier);

        if (currentPalier >= 2 && !timerActive)
        {
            timerActive = true;
            Invoke("TimeOut", timer);
        }

        isCooldownActive = false;
    }

    private void AdjustPalierValues(int palier)
    {
        float levelUpIncrement = 1.0f;

        spawnRadius = palier * 4;

        if (palier == 1)
        {
            spawnCount = 6;
        }
        else
        {
            spawnCount = 6 + ((palier - 1) * 6);
        }

        float newLevelUpValue = palier * levelUpIncrement;
    }

}
