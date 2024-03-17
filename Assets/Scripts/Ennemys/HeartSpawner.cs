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
    private int maxPalier = 5;
    public float timeNextPalier;
    public float timeSincePalierStart = 0f;


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
    public int cubesToLaunch;
    public float cubePatternTimer = 0f;
    public float cubePatternInterval = 10f;

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

    [Header("Cage Tracking Properties")]
    public float cageRadius;
    private bool cagePatternActive = false;
    private float cageTimer = 0f;
    public float cageTriggerTime;
    public float cageSpawnTime;
    public float cageTransparentScale;

    [Header("Patterns Properties")]
    [SerializeField] private PatternState currentPatternState;
    public float timeBetweenPatterns; // Temps entre chaque changement de pattern (en secondes)
    private float patternTimer = 0f;
    public CubeTracking cubeTrackingScript;


    private void Start()
    {
        cubeLauncherPattern = new CubeLauncherPattern();
        cubeLauncherPattern.heartSpawner = this;
        wallPattern = new WallPattern();
        StartCoroutine(SpawnCube());
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Idle", GetComponent<Transform>().position);
        BreakingHeart = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/BreakingTheHeart");
        BreakingHeart.start();

        //condition pour changement de state et appel de fonction pour lancer le pattern en question

        if (currentPalier == 1)
        {
            currentPatternState = PatternState.CubeTracking;
            StartCoroutine(StartCubeTrackingPattern());
        }
        else
        {
            currentPatternState = PatternState.CubeLauncher;
            StartCoroutine(StartCubeLauncherPattern());
        }
    }

    public enum PatternState
    {
        CubeTracking,
        CubeLauncher,
        CageTracking,
        //state à ajouter ici
    }

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
                //timerText.text = Mathf.Round(timer).ToString() + "s";
            }
        }

        // Condition pour déclencher le changement de palier automatique
        timeSincePalierStart += Time.deltaTime;
        if (timeSincePalierStart >= timeNextPalier) // 30 secondes
        {
            StartCoroutine(ChangePalierAutomatically());
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerPosition = playerObject.transform.position;
        }

        BreakingHeart.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        // Condition for spawn of the CageTracking pattern
        if (currentPalier >= 3 && !cagePatternActive && Vector3.Distance(playerPosition, transform.position) < (spawnRadius * cageRadius))
        {
            cageTimer += Time.deltaTime;

            if (cageTimer >= cageTriggerTime)
            {
                StartCoroutine(GenerateCagePattern());
            }
        }
        else
        {
            cageTimer = 0f; // Reset the timer if the player exits the zone
        }

        // Condition pour changer d'état après le délai spécifié
        patternTimer += Time.deltaTime;
        if (patternTimer >= timeBetweenPatterns)
        {
            patternTimer = 0f;  // Réinitialise le compteur de temps
            SwitchToNextPattern();
        }
    }

    //Fonction a changer pour les changements de patterns (les states)
    private void SwitchToNextPattern()
    {
        switch (currentPatternState)
        {
            case PatternState.CubeTracking:
                if (currentPalier == 1)  // Ajoutez cette condition pour le premier palier
                {
                    StartCoroutine(StartCubeLauncherPattern());
                    currentPatternState = PatternState.CubeTracking; // Reste dans le même état
                }
                else
                {
                    StartCoroutine(StartCubeTrackingPattern());
                    currentPatternState = PatternState.CubeLauncher; // Passe à l'état suivant
                }
                break;

            case PatternState.CubeLauncher:
                StartCoroutine(StartCubeLauncherPattern());
                currentPatternState = PatternState.CubeTracking; // Passe à l'état suivant
                break;

            case PatternState.CageTracking:
                //StartCoroutine(GenerateCagePattern());
                currentPatternState = PatternState.CageTracking; // Reste dans le même état
                break;

                // Ajoutez d'autres cas pour d'autres états au besoin
        }
    }



    private IEnumerator StartCubeTrackingPattern()
    {
        // Vérifier le palier avant de lancer le pattern
        if (currentPalier > 1)
        {
            // Appelez la fonction StartHomingCubePattern ici
            cubeTrackingScript.LaunchHomingCubes();
        }

        yield return null;
    }


    private IEnumerator StartCubeLauncherPattern()
    {

        // Appel de la méthode LauncherPattern manuellement
        cubeLauncherPattern.LauncherPattern();

        yield return null;
    }

    //creer fonction IEnumerator pour lancer ton pattern

    #region CUBES SPAWN
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
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
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
        //GameObject transparentCube = transparentCubesPool.GetObject(); nstantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
        // transparentCube.transform.position = spawnPosition;
        // transparentCube.transform.setParent(spawnContainer.transform);
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
    #endregion

    #region PALIER BEHAVIOURS
    public void ChangePalierOnTeleport()
    {
        if (isCooldownActive || currentPalier >= maxPalier)
            return;

        if (heartHealth != null)
        {
            timer = defaultTimer;

            float newLevelUpValue = (currentPalier + 1) * 1.0f;
            BreakingHeart.setParameterByName("LevelUp", newLevelUpValue);

            timerActive = true;
            StartCoroutine(ResetPalier());
            timeSincePalierStart = 0f;
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

        previousPalier = currentPalier;

        for (int palier = 1; palier <= currentPalier; palier++)
        {
            float temporarySpawnCountBeforeAdjust = spawnCount;
            AdjustPalierValues(palier);
            spawnCount = temporarySpawnCountBeforeAdjust;
            yield return new WaitForSeconds(timeTemporaryPalier);
        }

        // Restaurer les valeurs originales
        spawnRadius = originalSpawnRadius;
        spawnCount = originalSpawnCount;
        spawnInterval = originalSpawnInterval;

        // Passer au palier suivant
        currentPalier++;
        if (currentPalier <= maxPalier)
        {
            AdjustPalierValues(currentPalier);
        }

        if (currentPalier >= maxPalier && !timerActive)
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
            currentPatternState = PatternState.CageTracking;
            //cubeTrackingScript.numberOfCubesToLaunch = 30;
        }
        else if (palier == 2)
        {
            spawnCount = 6 + ((palier - 1) * 6);
            currentPatternState = PatternState.CubeLauncher;

        }
       
        else
        {
            // Ajoutez des cas pour d'autres paliers si nécessaire
        }

        float newLevelUpValue = palier * levelUpIncrement;
    }


    private IEnumerator ChangePalierAutomatically()
    {
        if (currentPalier < maxPalier)
        {
            // Augmenter le palier automatiquement
            currentPalier++;
            AdjustPalierValues(currentPalier);

            // Réinitialiser le temps pour le nouveau palier
            timeSincePalierStart = 0f;

            // Attendre avant de déclencher à nouveau le changement automatique (30 secondes)
            yield return new WaitForSeconds(timeNextPalier);

            // Appeler récursivement la fonction pour le palier suivant
            StartCoroutine(ChangePalierAutomatically());
        }
        else
        {
            // Si nous avons atteint le dernier palier, activer le chronomètre pour le timeout
            timerActive = true;
            Invoke("TimeOut", timer);
        }
    }

    #endregion

    #region BOSS PATTERNS
    private IEnumerator GenerateCagePattern()
    {
        cagePatternActive = true;

        Vector3 playerGridPosition = playerPosition / gridSize;
        playerGridPosition = new Vector3(Mathf.Round(playerGridPosition.x), Mathf.Round(playerGridPosition.y), Mathf.Round(playerGridPosition.z));
        playerGridPosition *= gridSize;

        int cageSizeXZ = 3;

        // Générer un seul cube transparent à une échelle plus grande
        GameObject transparentCube = Instantiate(transparentCubePrefab, playerGridPosition, Quaternion.identity, spawnContainer.transform);
        transparentCube.transform.localScale = new Vector3(cageTransparentScale, cageTransparentScale, cageTransparentScale);

        float timer = 0f;

        while (timer < cageSpawnTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Vérifier si le joueur est toujours dans le cube transparent
        float distanceToPlayer = Vector3.Distance(playerPosition, playerGridPosition);
        float acceptableOverlap = gridSize / 0.2f;  // plus la valeur est petite plus la cage se genera et sera clémente

        if (distanceToPlayer < (gridSize + acceptableOverlap))
        {
            // Générer la cage autour du joueur
            for (float x = playerGridPosition.x - cageSizeXZ; x <= playerGridPosition.x + cageSizeXZ; x += gridSize)
            {
                for (float y = playerGridPosition.y - cageSizeXZ; y <= playerGridPosition.y + cageSizeXZ; y += gridSize)
                {
                    for (float z = playerGridPosition.z - cageSizeXZ; z <= playerGridPosition.z + cageSizeXZ; z += gridSize)
                    {
                        Vector3 cageSpawnPosition = new Vector3(x, y, z);
                        if (Mathf.Abs(x - playerGridPosition.x) >= cageSizeXZ || Mathf.Abs(y - playerGridPosition.y) >= cageSizeXZ || Mathf.Abs(z - playerGridPosition.z) >= cageSizeXZ)
                        {
                            Instantiate(cubePrefab, cageSpawnPosition, Quaternion.identity, spawnContainer.transform);
                            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Cage/Traped");
                        }
                    }
                }
            }
        }

        // Détruire le cube transparent car la cage a été générée
        Destroy(transparentCube);

        cagePatternActive = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius * cageRadius);
    }
    #endregion

}
