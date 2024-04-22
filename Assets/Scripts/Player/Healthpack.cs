using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthpack : MonoBehaviour
{
    public int healthGain = 20;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerHealth = other.gameObject.transform.root.GetComponent<Player>();
            playerHealth.Heal(healthGain);
            Destroy(gameObject);
        }
    }
}
