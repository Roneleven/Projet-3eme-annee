using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternManager : MonoBehaviour
{
    private HeartSpawner heartSpawner;
    private CubeLauncherPattern cubeLauncherPattern;
    private CubeTracking cubeTrackingScript;
    private AerialMinesPattern aerialMinesPattern;
    private BigWallPattern bigWallPattern;
    private ExplosivePillarPattern explosivePillarPattern;
    private MeteorPattern meteorPattern;
    private GatlinLauncherPattern gatlinLauncherPattern;

    private bool playerInTrigger = false;
    public float detectionRadius = 30f; // Rayon de détection du joueur
    public float timer = 0f; // Temps écoulé depuis le début de la détection
    public bool patternActive = false;

    [Serializable]
    public class Palier
    {
        public List<PatternType> patterns;
    }

    public enum PatternType
    {
        CubeTracking,
        CubeLauncher,
        AerialMines,
        BigWall,
        ExplosivePillar,
        Meteor,
        GatlinLauncher
    }

    public List<Palier> paliers;
    public List<float> nextPatternTimes;
    private System.Random rand = new System.Random();

    private void Start()
    {
        heartSpawner = GetComponent<HeartSpawner>();
        cubeLauncherPattern = GetComponent<CubeLauncherPattern>();
        cubeTrackingScript = GetComponent<CubeTracking>();
        explosivePillarPattern = GetComponent<ExplosivePillarPattern>();
        bigWallPattern = GetComponent<BigWallPattern>();
        meteorPattern = GetComponent<MeteorPattern>();
        aerialMinesPattern = GetComponent<AerialMinesPattern>();
        gatlinLauncherPattern = GetComponent<GatlinLauncherPattern>();
        meteorPattern = GetComponent<MeteorPattern>();

        // Démarrez les patterns pour le premier palier
        StartPatternsForCurrentPalier();
    }

    private void Update()
    {
        // Vérifiez si le joueur est dans le rayon de détection et qu'aucun modèle n'est déjà actif
        if (!patternActive && Vector3.Distance(heartSpawner.playerPosition, transform.position) < detectionRadius)
        {
            // Incrémentation du timer
            timer += Time.deltaTime;

            // Obtenez le temps entre les patterns pour le palier actuel
            float patternTriggerTime = nextPatternTimes[heartSpawner.currentPalier];

            // Vérifiez si le temps de déclenchement est atteint
            if (timer >= patternTriggerTime)
            {
                // Démarrez un pattern aléatoire
                SwitchToNextPattern();
            }
        }
        else
        {
            // Réinitialisez le timer si le joueur n'est plus dans le rayon
            timer = 0f;
        }
    }

    public void SwitchToNextPattern()
    {
        // Obtenez le palier actuel
        int currentPalier = heartSpawner.currentPalier;

        // Obtenez les patterns pour ce palier
        List<PatternType> patternsForCurrentPalier = new List<PatternType>();
        patternsForCurrentPalier.AddRange(paliers[currentPalier].patterns);

        // Choisissez un pattern aléatoire dans la liste des patterns pour ce palier
        PatternType randomPattern = patternsForCurrentPalier[rand.Next(patternsForCurrentPalier.Count)];

        // Lancez le pattern choisi
        StartCoroutine(StartPattern(randomPattern));
    }

    public IEnumerator StartPattern(PatternType patternType)
    {
        patternActive = true;
        switch (patternType)
        {
            case PatternType.CubeTracking:
                cubeTrackingScript.LaunchHomingCubes();
                break;
            case PatternType.CubeLauncher:
                cubeLauncherPattern.LauncherPattern();
                break;
            case PatternType.AerialMines:
                aerialMinesPattern.LaunchAerialPattern();
                break;
            case PatternType.BigWall:
                bigWallPattern.LaunchWallPattern();
                break;
            case PatternType.ExplosivePillar:
                explosivePillarPattern.LaunchExplosivePillar();
                break;
            case PatternType.Meteor:
                meteorPattern.LaunchMeteorPattern();
                break;
            case PatternType.GatlinLauncher:
                gatlinLauncherPattern.SphereLauncherPattern();
                break;
        }
        yield return null; // Attendez la fin de l'exécution du modèle
        patternActive = false;
    }

    public void StartPatternsForCurrentPalier()
    {
        StopAllCoroutines();

        int currentPalier = heartSpawner.currentPalier;

        // Obtient les patterns pour ce palier
        List<PatternType> patternsForCurrentPalier = paliers[currentPalier].patterns;

        // Récupère la valeur nextPatternTime pour ce palier
        float interval = nextPatternTimes[currentPalier];

        // Lance les patterns en boucle avec l'intervalle correspondant
        StartCoroutine(LoopPatterns(patternsForCurrentPalier, interval));
    }


    private IEnumerator LoopPatterns(List<PatternType> patterns, float interval)
    {
        while (true)
        {
            if (playerInTrigger)
            {
                foreach (PatternType patternType in patterns)
                {
                    StartCoroutine(StartPattern(patternType));
                    yield return new WaitForSeconds(interval);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    public void PlayerEnteredTrigger()
    {
        playerInTrigger = true;
    }

    public void PlayerExitedTrigger()
    {
        playerInTrigger = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
