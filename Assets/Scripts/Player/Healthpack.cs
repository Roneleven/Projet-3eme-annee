using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthpack : MonoBehaviour
{
    public int healthGain = 20;
    private bool isCollected = false;
    public float respawnCollectibleTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isCollected)
        {
            Player playerHealth = other.gameObject.transform.root.GetComponent<Player>();
            playerHealth.Heal(healthGain);
            gameObject.SetActive(false);
            Invoke("RespawnCollectible", respawnCollectibleTime);
        }
    }

    void RespawnCollectible()
    {
        gameObject.SetActive(true);
        isCollected = false;
    }
}
