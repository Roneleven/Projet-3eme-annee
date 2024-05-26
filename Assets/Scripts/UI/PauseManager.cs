using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject CanvasPause;
    public FMOD.Studio.EventInstance backgroundMusic;
    private bool isPaused = false;

    private void Start()
    {
        //backgroundMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MARBLE ARCADE/SD_HUD/S_HUD_MUSIC/S_HUD_MUSIC_INGAME");
        //backgroundMusic.start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void ResumeGame()
    {
        CanvasPause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void PauseGame()
    {
        CanvasPause.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
