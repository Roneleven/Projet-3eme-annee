using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternManager : MonoBehaviour
{
    public HeartSpawner heartSpawner;
    public CubeLauncherPattern cubeLauncherPattern;
    public CubeTracking cubeTrackingScript;
    public AerialMinesPattern aerialMinesPattern;
    public BigWallPattern bigWallPattern;
    public ExplosivePillarPattern explosivePillarPattern;
    public MeteorPattern meteorPattern;
    public GatlinLauncherPattern gatlinLauncherPattern;
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
        // Ajoutez d'autres types de patterns si nécessaire
    }

    public List<Palier> paliers;
    private int currentPalierIndex = 0;
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

        paliers[0].patterns.Add(PatternType.CubeLauncher);
        paliers[0].patterns.Add(PatternType.CubeTracking);
        paliers[1].patterns.Add(PatternType.CubeLauncher);
        paliers[1].patterns.Add(PatternType.BigWall);
        paliers[2].patterns.Add(PatternType.ExplosivePillar);
        paliers[3].patterns.Add(PatternType.GatlinLauncher);
        paliers[4].patterns.Add(PatternType.Meteor);
        paliers[5].patterns.Add(PatternType.AerialMines);
        paliers[6].patterns.Add(PatternType.CubeTracking);
        paliers[7].patterns.Add(PatternType.GatlinLauncher);
        paliers[8].patterns.Add(PatternType.AerialMines);
        paliers[9].patterns.Add(PatternType.AerialMines);
        paliers[10].patterns.Add(PatternType.AerialMines);

        // Démarrez les patterns pour le premier palier
        int currentPalier = heartSpawner.currentPalier;

        // Utilisez currentPalier pour obtenir le palier actuel
        Palier currentPalierData = paliers[currentPalier];
        StartPatternsForCurrentPalier();
    }

    public void SwitchToNextPattern()
    {
        // Obtenez le palier actuel depuis HeartSpawner
        int currentPalier = heartSpawner.currentPalier;

        // Obtenez les patterns pour ce palier
        List<PatternType> patternsForCurrentPalier = paliers[currentPalier].patterns;

        // Choisissez un pattern aléatoire dans la liste des patterns pour ce palier
        PatternType randomPattern = patternsForCurrentPalier[rand.Next(patternsForCurrentPalier.Count)];

        // Lancez le pattern choisi
        StartCoroutine(StartPattern(randomPattern));

        // Vous pouvez ajouter d'autres logiques ici, par exemple
        // si vous voulez mettre à jour d'autres éléments de jeu
        // en fonction du pattern choisi
    }

    public IEnumerator StartPattern(PatternType patternType)
    {
        // Démarrez le pattern approprié en fonction du type de pattern
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
        // Obtenez le palier actuel depuis HeartSpawner
        int currentPalier = heartSpawner.currentPalier;

        // Obtenez les patterns pour ce palier
        List<PatternType> patternsForCurrentPalier = paliers[currentPalier].patterns;

        // Lancez les patterns en boucle toutes les 5 secondes
        StartCoroutine(LoopPatterns(patternsForCurrentPalier));
    }

    private IEnumerator LoopPatterns(List<PatternType> patterns)
    {
        while (true)
        {
            foreach (PatternType patternType in patterns)
            {
                StartCoroutine(StartPattern(patternType));
                yield return new WaitForSeconds(5f);
            }
        }
    }


}
