using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public GameObject debugMenuUI;
    public Toggle canMoveToggle;
    public HeartHealth heartHealth;
    public PlayerMovementsRB playerMovements;
    public List<Button> pallierButtons; // List of buttons for each pallier
    public List<Transform> pallierTransforms; // List of transforms for each pallier
    public HeartSpawner heartSpawner; // Reference to HeartSpawner script
    public GameObject theHeart;

    private bool debugMenuActive = false;

    void Start()
    {
        debugMenuUI.SetActive(false);
        canMoveToggle.onValueChanged.AddListener(OnCanMoveToggleChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            theHeart.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            debugMenuActive = !debugMenuActive;
            debugMenuUI.SetActive(debugMenuActive);
            Debug.Log($"Debug menu is now {(debugMenuActive ? "active" : "inactive")}");
        }
    }

    void OnCanMoveToggleChanged(bool isOn)
    {
        playerMovements.canMove = isOn;
        Debug.Log($"Player can move: {isOn}");
    }

    public void TeleportHeartToPosition1()
    {
        heartHealth.SetTargetForTeleportIndex(0); // L'index 0 correspond au premier point de t�l�portation
    }

    // M�thode appel�e lorsque le bouton de t�l�portation 2 est cliqu�
    public void TeleportHeartToPosition2()
    {
        heartHealth.SetTargetForTeleportIndex(1); // L'index 1 correspond au deuxi�me point de t�l�portation
    }

    // M�thode appel�e lorsque le bouton de t�l�portation 3 est cliqu�
    public void TeleportHeartToPosition3()
    {
        heartHealth.SetTargetForTeleportIndex(2); // L'index 2 correspond au troisi�me point de t�l�portation
    }
}
