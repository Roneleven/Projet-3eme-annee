using UnityEngine;

public class MachinegunCollectible : MonoBehaviour
{
    public GunController gunController; 

    [Header("Gun Properties")]
    public int newMaxAmmo;
    public float newFireRate;
    public float newRecoilForce;
    public float newReloadTime;
    public float recoilForce;
    public string newShootingSoundEvent = "event:/Guns/GatlinGun/Shoot";
    public string newReloadSoundEvent = "event:/Guns/GatlinGun/Reload";
    public bool newMustUseAllAmmoBeforeReload = false;

    [Header("Shooting Mechanics")]
    public int newBulletsPerShot ;
    public float newSpreadAmount;

    [Header("Bullet Settings")]
    public float newBulletSpeed;
    public float newBulletLifeTime;
    public int newBulletDamage;
    public int newBulletPenetrationCount;

    [Header("Explosive Settings")]
    public bool explosiveBullet = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyChangesToGun();
            FMODUnity.RuntimeManager.PlayOneShot("event:/LevelDesign/Collectibles/Collected");
            Destroy(gameObject);
        }
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
    }
}