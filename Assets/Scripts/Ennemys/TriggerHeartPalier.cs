using UnityEngine;

public class TriggerHeartPalier : MonoBehaviour
{
    public int palierToActivate;
    public HeartSpawner heartSpawner;

    private void Start()
    {
        heartSpawner = FindObjectOfType<HeartSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            heartSpawner.ActivatePalier(palierToActivate);
            Destroy(gameObject); // Optionally destroy the trigger after activation
        }
    }
}
