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
        int currentPalier = heartSpawner.currentPalier;

        // Utilisez currentPalier pour obtenir le palier actuel
        Palier currentPalierData = paliers[currentPalier];
        StartPatternsForCurrentPalier();
    }

    public void SwitchToNextPattern()
    {
        // Obtenez le palier actuel
        int currentPalier = heartSpawner.currentPalier;

        // Obtenez les patterns pour ce palier
        List<PatternType> patternsForCurrentPalier = new List<PatternType>();
        patternsForCurrentPalier.AddRange(paliers[currentPalier].patterns);

        // exclusion
        Transform tp = FindObjectOfType<HeartHealth>().getCurrentTeleportPoint();
        List<PatternType> tpPointExlusionList = tp.GetComponent<TeleportPoint>().exclusionList;

        foreach (PatternType pattern in tpPointExlusionList)
        {
            patternsForCurrentPalier.Remove(pattern);
        }

        // Choisissez un pattern aléatoire dans la liste des patterns pour ce palier
        PatternType randomPattern = patternsForCurrentPalier[rand.Next(patternsForCurrentPalier.Count)];

        // Lancez le pattern choisi
        StartCoroutine(StartPattern(randomPattern));

    }

    public IEnumerator StartPattern(PatternType patternType)
    {
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
        yield return null;
    }

    public void StartPatternsForCurrentPalier()
    {
        // Efface toutes les coroutines actives pour les patterns des paliers précédents
        StopAllCoroutines();

        // Obtient le palier actuel depuis HeartSpawner
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
            foreach (PatternType patternType in patterns)
            {
                StartCoroutine(StartPattern(patternType));
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
