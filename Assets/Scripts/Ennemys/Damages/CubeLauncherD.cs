using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncherD : MonoBehaviour
{
    

    public Player playerScript;

    private void Start()
    {
        // Chercher le script Player dans la scène
        playerScript = FindObjectOfType<Player>();

        if (playerScript == null)
        {
            Debug.LogError("Le script Player n'a pas été trouvé dans la scène.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerScript != null)
            {
                playerScript.TakeDamage(20);
            }
            else
            {
                Debug.LogError("Le script Player n'a pas été assigné à NCubeDamage.");
            }

            Destroy(gameObject);
        }
    }
}
