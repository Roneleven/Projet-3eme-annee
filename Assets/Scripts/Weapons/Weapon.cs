using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public enum FireMode
{
    Normal,
    Explosive,
    Laser
}
public class Weapon : MonoBehaviour
{
    public FireMode currentMode = FireMode.Normal;
    [Header("Damage")]
    public int damage;

    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Pickup")]
    public float animTime;

    [Header("Shooting")]
    public int shotsPerSecond;
    public float hitForce;
    public float range;
    public bool tapable;
    public float kickbackForce;
    public float resetSmooth;
    public Vector3 scopePos;
    public float spreadAngle; // New parameter for controlling spread angle


    [Header("ShootingVFX")]
    public ParticleSystem bulletTrailVFX;
    public VisualEffect muzzleFlash;
    public Transform muzzleFlashSpawnPoint;
    private VisualEffect muzzleFlashInstance;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject[] weaponGfxs;
    public Collider[] gfxColliders;
    public GameObject explosion;

    private float _rotationTime;
    private float _time;
    private bool _held;
    private bool _scoping;
    private bool _shooting;
    private Rigidbody _rb;
    private Transform _playerCamera;
    private TMP_Text _ammoText;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    public Recoil Recoil_Script;


    [Header("Explosive Mode")]
    public int explosiveDamage;
    public float explosiveRange;
    public float explosionRadius;
    public GameObject explosionPrefab;
    public int maxExplosiveCharges = 5;
    public int currentExplosiveCharges = 5; 
    private TMP_Text explosiveChargeText;
    private float chargeStartTime;
    public float chargeTimeThreshold;
    private int destroyedCubeCount = 0;
    public int cubesToDestroyToGainACharge = 10;
    public VisualEffect chargingEffect;
    public Transform chargingEffectSpawnPoint;
    private VisualEffect chargingEffectInstance;
    public VisualEffect explosiveEffect;
    private VisualEffect explosiveInstance;
    public ParticleSystem explosiveTrailVFX;


    [Header("Laser Mode")]
    public float laserCooldown = 1f;
    public float laserDuration = 1f;
    public float laserWidth = 3f;
    public float laserRange;
    private TMP_Text _laserText;
    private bool canShootLaser = true;
    public GameObject laserVFX;
    public GameObject laserSpawnPoint;
    public PlayerMovementsRB playerMovementsRB;
    public MouseLook mouseLookScript;
    private bool isChargingLaser = false;
    private bool isLaserActive = false;

    public float heatPerShot;
    public float overheatThreshold;
    public float cooldownTime;
    public float currentHeat; 
    public bool overheated;
    private float cooldownStartTime;
    public float cooldownRate;

    [Header("Heat UI")]
    public Image heatImage;
    public float maxFillAmount = 1f;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        Recoil_Script = transform.Find("FPS Player Gun Rework/CameraRot/CameraRecoil").GetComponent<Recoil>();
        currentExplosiveCharges = maxExplosiveCharges;
        currentMode = FireMode.Normal;
        mouseLookScript = playerMovementsRB.GetComponentInChildren<MouseLook>();
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
            _scoping = Input.GetMouseButton(1);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _scoping ? scopePos : Vector3.zero, resetSmooth * Time.deltaTime);
        }

        //tir clique gauche normal
        if ((tapable ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && !_shooting &&  currentMode == FireMode.Normal)
        {
            
            Shoot();
            StartCoroutine(ShootingCooldown());
        }

        //switch de mode

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToNormalMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToExplosiveMode();
        } else if (Input.GetKeyDown(KeyCode.Alpha3)){
            SwitchToLaserMode();
        }

        //tir clique gauche explosive

        if (currentMode == FireMode.Explosive)
{
    if (Input.GetMouseButtonDown(0) && currentExplosiveCharges > 0)
    {
        chargeStartTime = Time.time;
        chargingEffectInstance = Instantiate(chargingEffect, chargingEffectSpawnPoint.position, chargingEffectSpawnPoint.rotation);
        chargingEffectInstance.Play();
    }

    if (Input.GetMouseButton(0) && Time.time - chargeStartTime >= chargeTimeThreshold)
    {
        
                
    }

    if (Input.GetMouseButtonUp(0) && Time.time - chargeStartTime >= chargeTimeThreshold && currentExplosiveCharges > 0)
    {
        Vector3 shotDirection = _playerCamera.forward;
                if (!_scoping)
                {
                    Vector3 spreadDirection = Quaternion.Euler(Random.insideUnitSphere * spreadAngle) * shotDirection;
                    shotDirection = Vector3.Slerp(shotDirection, spreadDirection, 0.5f); // Adjust spread strength
                }
                
        explosiveInstance = Instantiate(explosiveEffect, chargingEffectSpawnPoint.position, chargingEffectSpawnPoint.rotation);
        explosiveInstance.Play();
        ExplosiveShoot();
        explosiveChargeText.text = "Charges: " + currentExplosiveCharges;

        if (explosiveTrailVFX != null)
                {
                    // Apply rotation to the particle system
                    Quaternion lookRotation = Quaternion.LookRotation(shotDirection, Vector3.up);
                    explosiveTrailVFX.transform.rotation = lookRotation;

                    // Play the particle system
                    explosiveTrailVFX.Play();
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Guns/BasicGun/Shoot");
                }
    }

    if (Input.GetMouseButtonUp(0))
    {
            chargingEffectInstance.Stop();
            Destroy(chargingEffectInstance.gameObject);
    }

    // Assure-toi que le VisualEffectInstance reste attaché au point de spawn de l'arme
    if (chargingEffectInstance != null)
    {
        chargingEffectInstance.transform.position = chargingEffectSpawnPoint.position;
        chargingEffectInstance.transform.rotation = chargingEffectSpawnPoint.rotation;
    }
    
    if (explosiveInstance != null)
    {
        explosiveInstance.transform.position = chargingEffectSpawnPoint.position;
        explosiveInstance.transform.rotation = chargingEffectSpawnPoint.rotation;
    }
}

        //LASER

        if (Input.GetMouseButton(0) && currentMode == FireMode.Laser && canShootLaser)
        {
            StartChargingLaser();
        }

        if (!overheated && !_shooting)
        {
            currentHeat -= cooldownRate * Time.deltaTime;

            // Clamp pour que ce soit entre 0 et 1
            currentHeat = Mathf.Clamp01(currentHeat);
        }

        if (overheated && Time.time - cooldownStartTime >= cooldownTime)
        {
            overheated = false;
            currentHeat = 0f; // Réinitialiser la chaleur à zéro lorsque l'arme n'est plus surchauffée
        }
        UpdateHeatUI();

        if (muzzleFlashInstance != null)
        {
            muzzleFlashInstance.transform.position = muzzleFlashSpawnPoint.position;
            muzzleFlashInstance.transform.rotation = muzzleFlashSpawnPoint.rotation;
        }
    }

    private void Shoot()
    {
        if (overheated)
        {
            if (Time.time - cooldownStartTime >= cooldownTime)
            {
                overheated = false;
                currentHeat = 0f;
            }
            else
            {
                return;
            }
        }
        if (currentHeat < overheatThreshold)
        {
            if (currentMode == FireMode.Normal)
            {
                Vector3 shotDirection = _playerCamera.forward;
                if (!_scoping)
                {
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
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Guns/BasicGun/Shoot");

                    muzzleFlashInstance = Instantiate(muzzleFlash, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
                    muzzleFlashInstance.Play();
                    muzzleFlashInstance.gameObject.AddComponent<VFXAutoDestroy>();
                }

                if (muzzleFlashInstance != null)
                {
                    muzzleFlashInstance.transform.position = muzzleFlashSpawnPoint.position;
                    muzzleFlashInstance.transform.rotation = muzzleFlashSpawnPoint.rotation;
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
            currentHeat += heatPerShot;
            if (currentHeat >= overheatThreshold)
            {
                // Si l'arme est en surchauffe, active le drapeau et commence le temps de refroidissement
                overheated = true;
                cooldownStartTime = Time.time;
            }
        }
    }


    private void UpdateHeatUI()
    {
        // Calculate the fill amount based on the current heat value and overheat threshold
        float fillAmount = Mathf.Clamp01(currentHeat / overheatThreshold);

        // Scale the fill amount to match the maximum fill amount
        fillAmount *= maxFillAmount;

        // Set the fill amount of the heat UI image
        if (heatImage != null)
        {
            heatImage.fillAmount = fillAmount;
        }
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

        if (cubeHealth != null && cubeHealth.health <= 0)
        {
            destroyedCubeCount++;

            if (destroyedCubeCount >= cubesToDestroyToGainACharge)
            {
                GainExplosiveCharge(); 
                destroyedCubeCount = 0;
            }
        }

        if (hitInfo.transform.CompareTag("ExplosiveBlock"))
        {
            Instantiate(explosion, hitInfo.transform.position, Quaternion.identity);
            Destroy(hitInfo.transform.gameObject);
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


    public void Pickup(Transform weaponHolder, Transform playerCamera, TMP_Text ammoText, TMP_Text chargeText, TMP_Text laserText)
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
        explosiveChargeText = chargeText;
        explosiveChargeText.text = "Charges: " + currentExplosiveCharges;
        _laserText = laserText;
        _laserText.text = "Laser";
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
        transform.parent = null;
        _held = false;
    }

    #region SWITCH TYPE ARMES
    public void SwitchFireMode()
    {
        currentMode = (currentMode == FireMode.Normal) ? FireMode.Explosive : FireMode.Normal;
    }

    private void SwitchToNormalMode()
    {
        currentMode = FireMode.Normal;
    }

    private void SwitchToExplosiveMode()
    {
        currentMode = FireMode.Explosive;
    }
    private void SwitchToLaserMode()
    {
        currentMode = FireMode.Laser;
    }

    #endregion

    #region MODE EXPLOSIVE
    private void ExplosiveShoot()
    {
        if (currentMode == FireMode.Explosive && currentExplosiveCharges > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, explosiveRange))
            {
                Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
                foreach (Collider hitCollider in colliders)
                {
                    CubeHealth cubeHealth = hitCollider.GetComponent<CubeHealth>();
                    if (cubeHealth != null)
                    {
                        cubeHealth.TakeDamage(explosiveDamage);
                    }
                }
            }

            currentExplosiveCharges--;
            explosiveChargeText.text = "Charges: " + currentExplosiveCharges;     
        }
    }

    public void GainExplosiveCharge()
    {
        if (currentExplosiveCharges < maxExplosiveCharges)
        {
            currentExplosiveCharges++;
            explosiveChargeText.text = "Charges: " + currentExplosiveCharges;
        }
    }
    #endregion

    #region MODE LASER

    private void StartChargingLaser()
    {
        if (!isChargingLaser && !isLaserActive)
        {
            StartCoroutine(ChargeLaser());
        }
    }

    private IEnumerator ChargeLaser()
    {
        isChargingLaser = true;
        float chargeStartTime = Time.time;

        // Ralentir le joueur en ajustant sa vitesse
        float originalSpeed = playerMovementsRB.speed;
        playerMovementsRB.speed *= 0.5f; // Réduire la vitesse à la moitié

        // Attendre pendant que le laser se charge
        while (Time.time - chargeStartTime < 2f)
        {
            yield return null;
        }

        // Rétablir la vitesse du joueur
        playerMovementsRB.speed = originalSpeed;

        isChargingLaser = false;
        FireLaser();
    }

    private void FireLaser()
    {
        if (currentMode == FireMode.Laser)
        {
            StartCoroutine(FireLaserCoroutine());
        }
    }


    private IEnumerator FireLaserCoroutine()
    {
        canShootLaser = false;
        isLaserActive = true;

        // Désactiver le script MouseLook
        mouseLookScript.enabled = false;

        // Activer le isKinematic
        playerMovementsRB.rb.isKinematic = true;

        // Instantiation du VFX de laser à la position et rotation du cannon du joueur
        GameObject laserInstance = Instantiate(laserVFX, laserSpawnPoint.transform.position, laserSpawnPoint.transform.rotation);

        float startTime = Time.time;

        while (Time.time - startTime < laserDuration)
        {
            // Lance un raycast pour détecter les collisions
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, laserWidth / 2f, transform.forward, laserRange);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("HeartBlock"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }

            // Positionne le VFX de laser à l'extrémité du rayon du laser
            Vector3 laserEnd = transform.position + transform.forward ;
            laserInstance.transform.position = laserEnd;

            // Met à jour la rotation du VFX de laser pour qu'il regarde dans la direction du rayon du laser
            laserInstance.transform.rotation = Quaternion.LookRotation(transform.forward);

            yield return null;
        }

        isLaserActive = false;

        // Réactiver le script MouseLook
        mouseLookScript.enabled = true;

        // Désactiver le isKinematic
        playerMovementsRB.rb.isKinematic = false;

        // Suppression du VFX de laser une fois que la durée est écoulée
        Destroy(laserInstance);
        StartCoroutine(LaserCooldown());
    }


    private IEnumerator LaserCooldown()
{
    yield return new WaitForSeconds(laserCooldown);
    canShootLaser = true;
    
        Debug.Log("laser");
}

    private void OnDrawGizmos()
    {
        // Dessine un gizmo représentant le rayon du laser
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * laserRange);
    }


    #endregion
    public bool Scoping => _scoping;
}