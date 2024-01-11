using UnityEngine;

public class RocketLauncherCollectible : MonoBehaviour
{
    public GunController gunController; // Faites glisser votre objet GunController ici depuis l'�diteur Unity

    [Header("Gun Properties")]
    public int newMaxAmmo;
    public float newFireRate;
    public float newRecoilForce;
    public float newReloadTime;
    public string newShootingSoundEvent = "event:/Guns/GrenadeGun/Shoot";
    public string newReloadSoundEvent = "event:/Guns/GrenadeGun/Reload";
    public bool newMustUseAllAmmoBeforeReload = false;
    private bool isCollected = false;
    public float respawnCollectibleTime;

    [Header("Shooting Mechanics")]
    public int newBulletsPerShot;
    public float newSpreadAmount;

    [Header("Bullet Settings")]
    public float newBulletSpeed;
    public float newBulletLifeTime;
    public int newBulletDamage;
    public int newBulletPenetrationCount;

    [Header("Exploslive Settings")]
    public bool explosiveBullet = true;
    public float newExplosionRadius;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            ApplyChangesToGun();
            FMODUnity.RuntimeManager.PlayOneShot("event:/LevelDesign/Collectibles/Collected");
            gameObject.SetActive(false);
            Invoke("RespawnCollectible", respawnCollectibleTime);
        }
    }

    void RespawnCollectible()
    {
        gameObject.SetActive(true);
        isCollected = false;
    }

    void ApplyChangesToGun()
    {
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
        gunController.explosiveEnabled = explosiveBullet;
        gunController.explosionRadius = newExplosionRadius;
    }
}