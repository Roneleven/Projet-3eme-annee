using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public GameObject debugMenuUI;
    public HeartHealth heartHealth;
    public Weapon weapon;
    public PlayerMovementsRB playerMovements;
    public List<Transform> palierTransforms;
    public HeartSpawner heartSpawner;
    public GameObject theHeart;
    public Transform weaponHolder;
    public Transform playerCamera;
    public Player player;

    private bool debugMenuActive = false;

    void Start()
    {
        debugMenuUI.SetActive(false);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugMenuActive = !debugMenuActive;

            theHeart.SetActive(debugMenuActive);
            debugMenuUI.SetActive(debugMenuActive);

            Cursor.lockState = debugMenuActive ? CursorLockMode.None : CursorLockMode.Locked;

            Time.timeScale = debugMenuActive ? 0 : 1;
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

    public void TeleportHeartToPosition4()
    {
        heartHealth.SetTargetForTeleportIndex(3);
        playerMovements.transform.position = palierTransforms[3].position;
    }

    public void TeleportHeartToPosition5()
    {
        heartHealth.SetTargetForTeleportIndex(4);
        playerMovements.transform.position = palierTransforms[4].position;
    }

    public void TeleportHeartToPosition6()
    {
        heartHealth.SetTargetForTeleportIndex(5);
        playerMovements.transform.position = palierTransforms[5].position;
    }

    public void PalierUp()
    {
        heartSpawner.ChangePalierOnTeleport();
    }

    public void pickUpWeapon()
    {   
        weapon.Pickup(weaponHolder, playerCamera); 
    }

    public void HeallFull()
    {
        player.Heal(100);
    }

    public void SpeedUp()
    {

    }
}
