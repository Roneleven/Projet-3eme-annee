using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanvasPause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        CanvasPause.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Arene_Demo");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
