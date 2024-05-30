using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject RetryButton;
    public GameObject MainMenuButton;
    private Player player;
    public static FMOD.Studio.EventInstance MenuMusique;
    void Start()
    {
        player = FindObjectOfType<Player>();
        MenuMusique = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/MenuBreakingTheHeart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Start");
        player.ResetLoomParameter();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Arene_Demo");
    }

    public void MainMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UX/Button/Select");
        player.ResetLoomParameter();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
        MenuMusique.start();
    }


}
