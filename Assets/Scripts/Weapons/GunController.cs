using UnityEngine;
using FMODUnity;

public class GunController : MonoBehaviour
{
    [Header("Gun Properties")]
    public int maxAmmo;
    public float fireRate;
    public float recoilForce;
    public float reloadTime;
    public string shootingSoundEvent = "event:/Guns/BasicGun/Shoot";
    public string reloadSoundEvent = "event:/Guns/BasicGun/Reload";
    public bool mustUseAllAmmoBeforeReload = false;

    [Header("Shooting Mechanics")]
    public Transform shootingPoint;
    public int bulletsPerShot;
    public float spreadAmount;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float bulletLifeTime;
    public int bulletDamage;
    public int bulletPenetrationCount;

    public int currentAmmo;
    private float nextTimeToFire;
    private bool isReloading = false;

    [Header("Explosive Settings")]
    public bool explosiveEnabled;
    public float explosionRadius;
    public GameObject explosionPrefab;

    [Header("Death Particle Settings")]
    public GameObject cubeDeath;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo >= bulletsPerShot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && (!mustUseAllAmmoBeforeReload || currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmo -= bulletsPerShot;
        FMODUnity.RuntimeManager.PlayOneShot(shootingSoundEvent, transform.position);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            if (bulletPrefab != null && shootingPoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

                // Ajout d'une balle explosive
                if (explosiveEnabled)
                {
                    bullet.AddComponent<ExplosiveBullet>().InitializeExplosive(explosionRadius, explosionPrefab, bulletDamage);
                }

                // Adding bullet spread
                Vector3 spread = Random.insideUnitSphere * spreadAmount;
                spread += shootingPoint.forward;
                Quaternion spreadRotation = Quaternion.LookRotation(spread);

                bullet.transform.rotation = spreadRotation;

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.InitializeBullet(bulletSpeed, bulletLifeTime, bulletDamage, bulletPenetrationCount, cubeDeath);

                }
            }
        }
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        FMODUnity.RuntimeManager.PlayOneShot(reloadSoundEvent, transform.position);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
