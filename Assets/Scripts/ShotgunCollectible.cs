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
                // Acc�der � toutes les variables publiques du GunController
                GunProperties gunProperties = gunController.gunProperties;

                // Modifier les propri�t�s du GunController
                gunProperties.maxAmmo = 7;
                gunProperties.fireRate = 200;
                gunProperties.bulletSpeed = 20;

                // D�truire le collectible
                Destroy(gameObject);
            }
        }
    }
}