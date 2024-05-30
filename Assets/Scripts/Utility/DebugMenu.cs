using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public GameObject debugMenuUI;
    public HeartHealth heartHealth;
    public Weapon weapon;
    public PlayerMovementsRB playerMovements;
    public List<Button> palierButtons;
    public List<Transform> palierTransforms;
    public HeartSpawner heartSpawner;
    public GameObject theHeart;
    public Transform weaponHolder;
    public Transform playerCamera;

    private bool debugMenuActive = false;

    void Start()
    {
        debugMenuUI.SetActive(false);
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

    public void TeleportHeartToPosition1()
    {
        heartHealth.SetTargetForTeleportIndex(0);
        playerMovements.transform.position = palierTransforms[0].position;
    }

    public void TeleportHeartToPosition2()
    {
        heartHealth.SetTargetForTeleportIndex(1);
        playerMovements.transform.position = palierTransforms[1].position;
    }

    public void TeleportHeartToPosition3()
    {
        heartHealth.SetTargetForTeleportIndex(2);
        playerMovements.transform.position = palierTransforms[2].position;
    }

    public void PalierUp()
    {
        heartSpawner.ChangePalierOnTeleport();
    }

    public void pickUpWeapon()
    {   
        weapon.Pickup(weaponHolder, playerCamera); 
    }
}
