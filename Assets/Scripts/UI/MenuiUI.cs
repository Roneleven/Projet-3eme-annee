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
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToOptions()
    {
        mainMenuUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainMenuUI.SetActive(true);
        optionsUI.SetActive(false);
    }
}