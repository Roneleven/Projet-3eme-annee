using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject CanvasPause;
    public FMOD.Studio.EventInstance backgroundMusic;
    private bool isPaused = false;
    private Player player;
    public static FMOD.Studio.EventInstance MenuMusique;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        MenuMusique = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/MenuBreakingTheHeart");
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Select");
        Cursor.lockState = CursorLockMode.Locked;
        CanvasPause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Back");
        Cursor.lockState = CursorLockMode.None;
        CanvasPause.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void RestartGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Start");
        player.ResetLoomParameter();
        SceneManager.LoadScene("Arene_Demo");
    }

    public void MainMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Select");
        player.ResetLoomParameter();
        SceneManager.LoadScene("Menu");
        MenuMusique.start();
    }

}
