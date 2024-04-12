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
    public int maxPalier = 10;
    public float timeNextPalier;
    public float timeSincePalierStart = 0f;
    public delegate void PalierChangeAction(int newPalier);
    public event PalierChangeAction OnPalierChange;


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
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;
    public int cubesToLaunch;
    public CubeLauncherPattern cubeLauncherPattern;

    [Header("Cage Tracking Properties")]
    public GameObject CageBlockPrefab;
    public float cageRadius;
    private bool cagePatternActive = false;
    private float cageTimer = 0f;
    public float cageTriggerTime;
    public float cageSpawnTime;
    public float cageTransparentScale;

    [Header("Patterns Properties")]
    public float timeBetweenPatterns; // Temps entre chaque changement de pattern (en secondes)
    private float patternTimer = 0f;
    public CubeTracking cubeTrackingScript;
    public AerialMinesPattern aerialMinesPattern;
    public BigWallPattern bigWallPattern;
    public ExplosivePillarPattern explosivePillarPattern;
    public MeteorPattern meteorPattern;
    public GatlinLauncherPattern gatlinLauncherPattern;

    public BossPatternManager bossPatternManager;
    //creer une classeliste de palier (donc 10), faire une boucle dans le start pour un new palier et inserer dans la liste
    //car la liste creer pas les objets tous seul, il faut en gros l'initialisé, en faite mettre en public
    //classe palier system seriazable, dans cette classe liste de pattern
    //creer enum de pattern (tous les pattern disponible) commec a palier font ref à x éléments de cet énum
    //switchcurrentpalier etc la fonction, prendre la liste de palier, indexer avec current palier, pour recup le palier et faire .pattern pour avoir la liste des patterns
    //on choisis un au hasard par exemple, une fois choisis on fait un switch qui va ressembler a ce que j'avais en un peu moins lourd
    // case bigwall,case cubelauncher etc si on est dans le case cubetracking on startcoroutine cubetracking, et c'est déclencher en fonction du palier ou on est
    //dans hearthealth mettre une list de pattern pour les exclusions, scrap la data et le code sera dans switch to next pattern en gros, c'est elle qui lance les pattern et recupere tout. random ou algo pour choisir qui appelle la bonne coroutine


    private void Start()
    {
        cubeLauncherPattern = GetComponent<CubeLauncherPattern>();
        explosivePillarPattern = GetComponent<ExplosivePillarPattern>();
        bigWallPattern = GetComponent<BigWallPattern>();
        meteorPattern = GetComponent<MeteorPattern>();
        aerialMinesPattern = GetComponent<AerialMinesPattern>();
        gatlinLauncherPattern = GetComponent<GatlinLauncherPattern>();

        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Idle", GetComponent<Transform>().position);
        BreakingHeart = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/CoreBreaked");
        BreakingHeart.start();

        StartCoroutine(SpawnCube());
        bossPatternManager.StartPatternsForCurrentPalier();
    }


    public void OnCurrentPalierChanged()
    {
        bossPatternManager.StartPatternsForCurrentPalier();
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
    }

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
                            Instantiate(CageBlockPrefab, cubePosition, Quaternion.identity, spawnContainer.transform);
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
            BreakingHeart.setParameterByName("LevelUp 2", newLevelUpValue);

            timerActive = true;
            StartCoroutine(ResetPalier());
            timeSincePalierStart = 0f;
            patternTimer = 0f;

            // Déclencher l'événement OnPalierChange avec le nouveau palier
            if (OnPalierChange != null)
            {
                OnPalierChange(currentPalier + 1);
            }
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
        OnCurrentPalierChanged();

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
        }
        else if (palier == 2)
        {
            spawnCount = 6 + ((palier - 1) * 4);

        }

        float newLevelUpValue = palier * levelUpIncrement;

        // Déclencher l'événement OnPalierChange avec le nouveau palier
        if (OnPalierChange != null)
        {
            OnPalierChange(palier);
        }
    }


    private IEnumerator ChangePalierAutomatically()
    {
        if (currentPalier < maxPalier)
        {
            currentPalier++;
            OnCurrentPalierChanged();

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
                            Instantiate(CageBlockPrefab, cageSpawnPosition, Quaternion.identity, spawnContainer.transform);
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