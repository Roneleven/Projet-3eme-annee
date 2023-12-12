using UnityEngine;
using FMODUnity;

public class GunController : MonoBehaviour
{
    [Header("Gun Properties")]
    public int maxAmmo = 30;
    public float fireRate = 0.5f;
    public float recoilForce = 1.0f;
    public float reloadTime = 2.0f;
    public string shootingSoundEvent = "event:/Player/Shoot";
    public string reloadSoundEvent = "event:/Player/Shoot";
    public bool mustUseAllAmmoBeforeReload = false;

    [Header("Shooting Mechanics")]
    public Transform shootingPoint;
    public int bulletsPerShot = 1;
    public float spreadAmount = 0.1f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 5f;
    public int bulletDamage = 10;
    public int bulletPenetrationCount = 1;

    public int currentAmmo;
    private float nextTimeToFire = 0.0f;
    private bool isReloading = false;

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

                // Adding bullet spread
                Vector3 spread = Random.insideUnitSphere * spreadAmount;
                spread += shootingPoint.forward;
                Quaternion spreadRotation = Quaternion.LookRotation(spread);

                bullet.transform.rotation = spreadRotation;

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(bulletSpeed, bulletLifeTime, bulletDamage, bulletPenetrationCount);
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
