using UnityEngine;

public class ShotgunCollectible : MonoBehaviour
{
    public GunController gunController; // Faites glisser votre objet GunController ici depuis l'�diteur Unity

    // Les nouvelles propri�t�s que vous souhaitez appliquer lorsque le collectible est ramass�
    [Header("Gun Properties")]
    public int newMaxAmmo = 50;
    public float newFireRate = 0.7f;
    public float newRecoilForce = 0.8f;
    public float newReloadTime = 1.5f;
    public float recoilForce = 1.0f;
    public string newShootingSoundEvent = "event:/Player/Shoot";
    public string newReloadSoundEvent = "event:/Player/Shoot";
    public bool newMustUseAllAmmoBeforeReload = false;

    [Header("Shooting Mechanics")]
    public int newBulletsPerShot = 1;
    public float newSpreadAmount = 0.1f;

    [Header("Bullet Settings")]
    public float newBulletSpeed = 100f;
    public float newBulletLifeTime = 5f;
    public int newBulletDamage = 1;
    public int newBulletPenetrationCount = 1;

    // Ajoutez d'autres propri�t�s selon vos besoins

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyChangesToGun();
            Destroy(gameObject); // D�truisez le collectible apr�s qu'il a �t� ramass�
        }
    }

    void ApplyChangesToGun()
    {
        // Appliquer les nouvelles propri�t�s au GunController
        gunController.maxAmmo = newMaxAmmo;
        gunController.fireRate = newFireRate;
        gunController.recoilForce = newRecoilForce;
        gunController.reloadTime = newReloadTime;
        gunController.shootingSoundEvent = newShootingSoundEvent;
        gunController.reloadSoundEvent = newReloadSoundEvent;
        gunController.mustUseAllAmmoBeforeReload = newMustUseAllAmmoBeforeReload;
        gunController.bulletsPerShot = newBulletsPerShot;
        gunController.spreadAmount = newSpreadAmount;
        gunController.bulletSpeed = newBulletSpeed;
        gunController.bulletLifeTime = newBulletLifeTime;
        gunController.bulletDamage = newBulletDamage;
        gunController.currentAmmo = Mathf.Min(gunController.currentAmmo, gunController.maxAmmo);
        // Appliquer d'autres propri�t�s selon vos besoins
    }
}