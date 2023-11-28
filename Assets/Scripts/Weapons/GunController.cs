using UnityEngine;
using FMODUnity;

public class GunController : MonoBehaviour
{
    [Header("Gun Properties")]
    public int maxAmmo = 30;
    public float fireRate = 0.5f;
    public float recoilForce = 1.0f;
    public float reloadTime = 2.0f;
    public string shootingSoundEvent = "event:/shootingSound";
    public string reloadSoundEvent = "event:/reloadSound";

    [Header("Shooting Mechanics")]
    public Transform shootingPoint;  // The point from where bullets are shot
    public GameObject bulletPrefab;  // The bullet prefab
    public int bulletsPerShot = 1;   // Number of bullets fired per shot

    private int currentAmmo;
    private float nextTimeToFire = 0.0f;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo >= bulletsPerShot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
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
                Vector3 spread = shootingPoint.forward;
                spread.x += Random.Range(-recoilForce, recoilForce) * 0.1f; // Adjust these values for more/less spread
                spread.y += Random.Range(-recoilForce, recoilForce) * 0.1f;
                bullet.transform.forward = spread.normalized;

                // Add force or velocity to the bullet if needed
                // bullet.GetComponent<Rigidbody>().AddForce(spread * bulletSpeed, ForceMode.VelocityChange);
            }
        }

        // Apply recoil and other effects here
    }

    System.Collections.IEnumerator Reload()
    {
        FMODUnity.RuntimeManager.PlayOneShot(reloadSoundEvent, transform.position);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }
}
