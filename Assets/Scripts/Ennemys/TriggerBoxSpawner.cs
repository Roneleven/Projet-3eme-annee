using UnityEngine;

public class TriggerBoxSpawner : MonoBehaviour
{
    public BoxSpawner[] boxSpawners; // Tableau de BoxSpawner

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var boxSpawner in boxSpawners)
            {
                boxSpawner.ActivateSpawner();
            }
            Destroy(gameObject); // Optionally destroy the trigger after activation
        }
    }
}
