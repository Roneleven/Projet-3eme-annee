using UnityEngine;

public class ShotgunCollectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GunController gunController = other.GetComponentInChildren<GunController>();

            if (gunController != null)
            {
                // Accéder à toutes les variables publiques du GunController
                GunProperties gunProperties = gunController.gunProperties;

                // Modifier les propriétés du GunController
                gunProperties.maxAmmo = 7;
                gunProperties.fireRate = 200;
                gunProperties.bulletSpeed = 20;

                // Détruire le collectible
                Destroy(gameObject);
            }
        }
    }
}