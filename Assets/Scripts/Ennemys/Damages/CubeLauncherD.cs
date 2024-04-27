using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherD : MonoBehaviour
{
    

    public Player playerScript;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
