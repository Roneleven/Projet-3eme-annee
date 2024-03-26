using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Damage")]
    public int damage;

    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Pickup")]
    public float animTime;

    [Header("Shooting")]
    public int maxAmmo;
    public int shotsPerSecond;
    public float reloadSpeed;
    public float hitForce;
    public float range;
    public bool tapable;
    public float kickbackForce;
    public float resetSmooth;
    public Vector3 scopePos;
    public float spreadAngle; // New parameter for controlling spread angle

    [Header("ShootingVFX")]
    public ParticleSystem bulletTrailVFX;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject[] weaponGfxs;
    public Collider[] gfxColliders;

    private float _rotationTime;
    private float _time;
    private bool _held;
    private bool _scoping;
    private bool _reloading;
    private bool _shooting;
    private int _ammo;
    private Rigidbody _rb;
    private Transform _playerCamera;
    private TMP_Text _ammoText;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    public Recoil Recoil_Script;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        _ammo = maxAmmo;
        Recoil_Script = transform.Find("FPS Player Gun Rework/CameraRot/CameraRecoil").GetComponent<Recoil>();
    }

    private void Update()
    {
        if (!_held) return;

        if (_time < animTime)
        {
            _time += Time.deltaTime;
            _time = Mathf.Clamp(_time, 0f, animTime);
            var delta = -(Mathf.Cos(Mathf.PI * (_time / animTime)) - 1f) / 2f;
            transform.localPosition = Vector3.Lerp(_startPosition, Vector3.zero, delta);
            transform.localRotation = Quaternion.Lerp(_startRotation, Quaternion.identity, delta);
        }
        else
        {
            _scoping = Input.GetMouseButton(1) && !_reloading;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _scoping ? scopePos : Vector3.zero, resetSmooth * Time.deltaTime);
        }

        if (_reloading)
        {
            _rotationTime += Time.deltaTime;
            var spinDelta = -(Mathf.Cos(Mathf.PI * (_rotationTime / reloadSpeed)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3(spinDelta * 360f, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.R) && !_reloading && _ammo < maxAmmo)
        {
            StartCoroutine(ReloadCooldown());
        }

        if ((tapable ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && !_shooting && !_reloading)
        {
            _ammo--;
            _ammoText.text = _ammo + " / " + maxAmmo;
            Shoot();
            StartCoroutine(_ammo <= 0 ? ReloadCooldown() : ShootingCooldown());
        }
    }

    private void Shoot()
    {
        // Reduce accuracy if not scoped
        Vector3 shotDirection = _playerCamera.forward;
        if (!_scoping)
        {
            // Add random deviation to the shot direction
            Vector3 spreadDirection = Quaternion.Euler(Random.insideUnitSphere * spreadAngle) * shotDirection;
            shotDirection = Vector3.Slerp(shotDirection, spreadDirection, 0.5f); // Adjust spread strength
        }

        // Apply kickback force
        transform.localPosition -= new Vector3(0, 0, kickbackForce);

        // Play bullet trail VFX regardless of hitting something or not
        if (bulletTrailVFX != null)
        {
            // Apply rotation to the particle system
            Quaternion lookRotation = Quaternion.LookRotation(shotDirection, Vector3.up);
            bulletTrailVFX.transform.rotation = lookRotation;

            // Play the particle system
            bulletTrailVFX.Play();
        }

        // Perform raycast to check for hit
        RaycastHit hitInfo;
        if (Physics.Raycast(_playerCamera.position, shotDirection, out hitInfo, range))
        {
            // Process hit object
            var heartHealth = hitInfo.transform.GetComponent<HeartHealth>();
            if (heartHealth != null)
            {
                heartHealth.TakeDamage(damage);
            }
            else
            {
                HandleHitObject(hitInfo);
            }
        }
        Recoil_Script.RecoilFire();
    }

    private void HandleHitObject(RaycastHit hitInfo)
    {
        var rb = hitInfo.transform.GetComponent<Rigidbody>();
        var cubeHealth = hitInfo.transform.GetComponent<CubeHealth>();

        if (rb != null)
        {
            rb.velocity += _playerCamera.forward * hitForce;
        }

        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(damage);

            if (cubeHealth.health <= 0)
            {
                if (!cubeHealth.IsDead())
                {
                    cubeHealth.SetDead(true);
                }

                Destroy(hitInfo.transform.gameObject);
            }
        }

        if (hitInfo.transform.CompareTag("DestroyableBlock"))
    {
        Destroy(hitInfo.transform.gameObject);
    }
    }

    private IEnumerator ShootingCooldown()
    {
        _shooting = true;
        yield return new WaitForSeconds(1f / shotsPerSecond);
        _shooting = false;
    }

    private IEnumerator ReloadCooldown()
    {
        _reloading = true;
        _ammoText.text = "RELOADING";
        _rotationTime = 0f;
        yield return new WaitForSeconds(reloadSpeed);
        _ammo = maxAmmo;
        _ammoText.text = _ammo + " / " + maxAmmo;
        _reloading = false;
    }

    public void Pickup(Transform weaponHolder, Transform playerCamera, TMP_Text ammoText)
    {
        if (_held) return;
        Destroy(_rb);
        _time = 0f;
        transform.parent = weaponHolder;
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        foreach (var col in gfxColliders)
        {
            col.enabled = false;
        }
        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = weaponGfxLayer;
        }
        _held = true;
        _playerCamera = playerCamera;
        _ammoText = ammoText;
        _ammoText.text = _ammo + " / " + maxAmmo;
        _scoping = false;
    }

    public void Drop(Transform playerCamera)
    {
        if (!_held) return;
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        var forward = playerCamera.forward;
        forward.y = 0f;
        _rb.velocity = forward * throwForce;
        _rb.velocity += Vector3.up * throwExtraForce;
        _rb.angularVelocity = Random.onUnitSphere * rotationForce;
        foreach (var col in gfxColliders)
        {
            col.enabled = true;
        }
        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = 0;
        }
        _ammoText.text = "";
        transform.parent = null;
        _held = false;
    }

    public bool Scoping => _scoping;
}