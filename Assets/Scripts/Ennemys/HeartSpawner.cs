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
    private float cocon;
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
    public GameObject coconvfx;

    private FMOD.Studio.EventInstance BreakingHeart;

    [Header("Timer/Reset Properties")]
    public float timer;
    public float defaultTimer;
    private bool timerActive = false;
    public TextMeshProUGUI timerText;
    public Image blackFade;
    public Animator anim;
    public Image timerFillImage; // New UI Image for timer representation

    private bool isCooldownActive = false;

    private bool playerInPosition;
    public Vector3 playerPosition;

    [Header("Throw cube pattern Properties")]
    public float cubeDestroyDelay;
    public float launchForce;
    public float percentageToLaunch;
    public int cubesToLaunch;

    [Header("Cage Tracking Properties")]
    public GameObject CageBlockPrefab;
    public float cageRadius;
    private bool cagePatternActive = false;
    private float cageTimer = 0f;
    public float cageTriggerTime;
    public float cageSpawnTime;
    public GameObject cagePatternPreview;
    public float cageTransparentScale;
    private FMOD.Studio.EventInstance warning;

    [Header("Patterns Properties")]
    public CubeLauncherPattern cubeLauncherPattern;
    public CubeTracking cubeTrackingScript;
    public AerialMinesPattern aerialMinesPattern;
    public BigWallPattern bigWallPattern;
    public ExplosivePillarPattern explosivePillarPattern;
    public MeteorPattern meteorPattern;
    public GatlinLauncherPattern gatlinLauncherPattern;

    public BossPatternManager bossPatternManager;

    private void Start()
    {
        cubeLauncherPattern = GetComponent<CubeLauncherPattern>();
        cubeTrackingScript = GetComponent<CubeTracking>();
        explosivePillarPattern = GetComponent<ExplosivePillarPattern>();
        bigWallPattern = GetComponent<BigWallPattern>();
        meteorPattern = GetComponent<MeteorPattern>();
        aerialMinesPattern = GetComponent<AerialMinesPattern>();
        gatlinLauncherPattern = GetComponent<GatlinLauncherPattern>();
        bossPatternManager = GetComponent<BossPatternManager>();
        heartHealth = GetComponent<HeartHealth>();
        warning = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/Cage_Warning");
        BreakingHeart = FMODUnity.RuntimeManager.CreateInstance("event:/V1/UX/Ambience/CoreBreaked");  // Integrate music here
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
                // Calculate percentage completed
                float percentageCompleted = (1 - (timer / defaultTimer)) * 100;
                timerText.text = Mathf.RoundToInt(percentageCompleted).ToString() + "%";

                if (timerFillImage != null)
                {
                    // Fill amount based on the percentage completed
                    timerFillImage.fillAmount = 1 - (timer / defaultTimer);
                }
            }
        }

        // Condition pour déclencher le changement de palier automatique
        timeSincePalierStart += Time.deltaTime;
        if (timeSincePalierStart >= timeNextPalier)
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
                                cubeHealth.UpdateMaterial();
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
                cubeHealth.UpdateMaterial();
                break;
            }
        }
    }

    private IEnumerator SpawnTransparentAndRealCube(Vector3 spawnPosition)
    {
        GameObject transparentCube = Instantiate(transparentCubePrefab, spawnPosition, Quaternion.identity, spawnContainer.transform);
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

            // Déclencher l'événement OnPalierChange avec le nouveau palier
            if (OnPalierChange != null)
            {
                OnPalierChange(currentPalier + 1);
            }
        }
    }

    private void UpdateCocon()
    {
        cocon = currentPalier * 0.2f + 2;
        coconvfx.transform.localScale = new Vector3(cocon, cocon, cocon);
    }

    private IEnumerator ResetPalier()
    {
        previousPalier = currentPalier;
        currentPalier++;
        UpdateCocon();

        // Augmenter le nombre de cubes à spawner pendant la durée temporaire
        spawnCount = temporarySpawnCount;
        spawnInterval = temporarySpawnInterval;

        // Attendre la durée temporaire
        yield return new WaitForSeconds(timeTemporaryPalier);

        // Revenir aux valeurs normales
        spawnCount = Mathf.RoundToInt(spawnCount * (1.2f + (currentPalier * 0.1f)));
        spawnInterval = spawnInterval * (0.9f - (currentPalier * 0.05f));

        // Notifier le changement de palier
        OnCurrentPalierChanged();
    }

    private IEnumerator ChangePalierAutomatically()
    {
        if (currentPalier < maxPalier)
        {
            currentPalier++;
            timeSincePalierStart = 0f;
            timeNextPalier += timeTemporaryPalier;

            float newLevelUpValue = (currentPalier + 1) * 1.0f;
            BreakingHeart.setParameterByName("LevelUp 2", newLevelUpValue);

            // Déclencher l'événement OnPalierChange avec le nouveau palier
            if (OnPalierChange != null)
            {
                OnPalierChange(currentPalier);
            }

            yield return new WaitForSeconds(timeTemporaryPalier);

            OnCurrentPalierChanged();
        }
    }
    #endregion

    #region CAGE TRACKING
    public IEnumerator GenerateCagePattern()
    {
        // Preview setup
        GameObject preview = Instantiate(cagePatternPreview, playerPosition, Quaternion.identity);
        preview.transform.localScale = Vector3.one * cageTransparentScale;
        preview.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f);
        warning.start();

        yield return new WaitForSeconds(cageSpawnTime);

        Destroy(preview);

        GameObject cage = Instantiate(CageBlockPrefab, playerPosition, Quaternion.identity);
        cage.transform.localScale = Vector3.one * cageRadius;
        cagePatternActive = true;

        yield return new WaitForSeconds(3f); // Duration of the pattern
        cagePatternActive = false;
    }
    #endregion
}
