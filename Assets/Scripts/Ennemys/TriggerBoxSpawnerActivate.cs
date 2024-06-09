using UnityEngine;

public class TriggerBoxSpawnerActivate : MonoBehaviour
{
    public BoxSpawnerActivateTrigger[] boxSpawners; // Tableau de BoxSpawnerActivateTrigger
    public HeartHealth heartHealth; // R�f�rence au script HeartHealth
    public HeartSpawner heartSpawner; // R�f�rence au script HeartHealth
    public int requiredPalier; // Le palier n�cessaire pour activer les boxSpawners
    public GameObject targetObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && heartHealth != null && heartSpawner.currentPalier == requiredPalier - 1)
        {
            foreach (var boxSpawner in boxSpawners)
            {
                boxSpawner.ActivateSpawner();
            }

            if (targetObject != null)
            {
                Destroy(targetObject);
            }
            Destroy(gameObject); // Optionally destroy the trigger after activation
        }
    }
}
