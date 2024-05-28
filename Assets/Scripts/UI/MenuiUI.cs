using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuiUI : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject optionsUI;

    public void ChangeScene(string sceneName)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Start");
        PauseManager.MenuMusique.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToOptions()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Select");
        mainMenuUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Back");
        mainMenuUI.SetActive(true);
        optionsUI.SetActive(false);
    }
}