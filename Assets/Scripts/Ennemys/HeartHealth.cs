using UnityEngine;

public class HeartHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public int currentPalier = 1;
    public Transform[] teleportPositions; // Tableau des positions de téléportation
    private int lastTeleportIndex = -1;
    private HeartSpawner heartSpawner;

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            TeleportHeart();
        }
    }

    private void TeleportHeart()
    {
        if (teleportPositions.Length > 0)
        {
            int newTeleportIndex;
            do
            {
                newTeleportIndex = Random.Range(0, teleportPositions.Length);
            } while (newTeleportIndex == lastTeleportIndex);
            lastTeleportIndex = newTeleportIndex;
            Transform nextTeleportPosition = teleportPositions[lastTeleportIndex];
            transform.position = nextTeleportPosition.position;

            health = maxHealth;

            // Ajouter cette ligne pour déclencher le changement de palier
            if (heartSpawner != null)
            {
                heartSpawner.ChangePalierOnTeleport();
            }
        }
    }
}
