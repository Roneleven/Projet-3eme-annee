using UnityEngine;
using FMODUnity;

[System.Serializable]
public class GunProperties
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
}

public class GunController : MonoBehaviour
{
    public GunProperties gunProperties;

    [HideInInspector]
    public int currentAmmo;

    private float nextTimeToFire = 0.0f;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = gunProperties.maxAmmo;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo >= gunProperties.bulletsPerShot)
        {
            nextTimeToFire = Time.time + 1f / gunProperties.fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && (!gunProperties.mustUseAllAmmoBeforeReload || currentAmmo < gunProperties.maxAmmo))
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmo -= gunProperties.bulletsPerShot;
        FMODUnity.RuntimeManager.PlayOneShot(gunProperties.shootingSoundEvent, transform.position);

        for (int i = 0; i < gunProperties.bulletsPerShot; i++)
        {
            if (gunProperties.bulletPrefab != null && gunProperties.shootingPoint != null)
            {
                GameObject bullet = Instantiate(gunProperties.bulletPrefab, gunProperties.shootingPoint.position, gunProperties.shootingPoint.rotation);

                // Adding bullet spread
                Vector3 spread = Random.insideUnitSphere * gunProperties.spreadAmount;
                spread += gunProperties.shootingPoint.forward;
                Quaternion spreadRotation = Quaternion.LookRotation(spread);

                bullet.transform.rotation = spreadRotation;

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(gunProperties.bulletSpeed, gunProperties.bulletLifeTime, gunProperties.bulletDamage, gunProperties.bulletPenetrationCount);
                }
            }
        }
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        FMODUnity.RuntimeManager.PlayOneShot(gunProperties.reloadSoundEvent, transform.position);
        yield return new WaitForSeconds(gunProperties.reloadTime);
        currentAmmo = gunProperties.maxAmmo;
        isReloading = false;
    }
}
