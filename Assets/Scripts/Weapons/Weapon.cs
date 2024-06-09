using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEditor.PackageManager;

public enum FireMode
{
    Normal,
    Explosive,
    //Laser
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
    public bool _held;
    private bool _scoping;
    private bool _shooting;
    private Rigidbody _rb;
    private Transform _playerCamera;
    private TMP_Text _ammoText;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    public Recoil Recoil_Script;
    private float originalSpeed;
    public PlayerMovementsRB playerMovementsRB;


    [Header("Explosive Mode")]
    public int explosiveDamage;
    public float explosiveRange;
    public float explosionRadius;
    public float kickbackForceExplosive;
    public float kickbackTimeExplosive;
    public GameObject explosionPrefab;
    public int maxExplosiveCharges = 5;
    public int currentExplosiveCharges = 5;
    public TMP_Text explosiveChargeText;
    private float chargeStartTime;
    public float chargeTimeThreshold;
    private int destroyedCubeCount = 0;
    public int cubesToDestroyToGainACharge = 5;
    public float chargingShakeIntensity = 0.1f;
    public AnimationCurve shakeCurve;

    /*[Header("Laser Mode")]
    public float laserCooldown = 1f;
    public float laserDuration = 1f;
    public float laserWidth = 3f;
    public float laserRange;
    private bool canShootLaser = true;
    public GameObject laserVFX;
    public GameObject laserSpawnPoint;
    
    public MouseLook mouseLookScript;
    private bool isChargingLaser = false;
    private bool isLaserActive = false;*/

    [Header("Heat Settings")]
    public float heatPerShot;
    public float heatEffectMultiplier;
    public float overheatThreshold;
    public float cooldownTime;
    public float currentHeat;
    public bool overheated;
    private float cooldownStartTime;
    public float cooldownRate;
    

    [Header("Heat UI")]
    public Image heatImage;
    public float maxFillAmount = 1f;

    [Header("VFXs")]
    public VisualEffect chargingEffect;
    public Transform chargingEffectSpawnPoint;
    private VisualEffect chargingEffectInstance;
    public VisualEffect explosiveEffect;
    private VisualEffect explosiveInstance;
    public ParticleSystem explosiveTrailVFX;
    public VisualEffect explosionEffect;
    private VisualEffect explosionInstance;
    public VisualEffect readyToFireEffect;
    private VisualEffect readyToFireInstance;
    public VisualEffect shootEffect;
    private VisualEffect shootInstance;
    public VisualEffect smokeEffect;
    public Transform smokeEffectSpawnPoint;
    private VisualEffect smokeInstance;
    public VisualEffect CubeHit;
    private VisualEffect CubeHitInstance;

    [Header("Mode Materials")]
    public GameObject weaponModel;
    public Material normalModeMaterial;
    public Material explosiveModeMaterial;
    public Material laserModeMaterial;

    [Header("UI Manager")]
    public GameObject weaponUIPanel;
    public Camera mainCamera;
    public Camera xRayCamera;
    public LayerMask normalViewMask;
    public LayerMask thermalViewMask;
    public GameObject targetObject1;
    public GameObject targetObject2;
    public float angleMargin = 5f;

    [Header("Bobbing Settings")]
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    private float bobbingTimer = 0f;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        Recoil_Script = transform.Find("FPS Player Gun Rework/CameraRot/CameraRecoil").GetComponent<Recoil>();
        currentExplosiveCharges = maxExplosiveCharges;
        currentMode = FireMode.Normal;
        //mouseLookScript = playerMovementsRB.GetComponentInChildren<MouseLook>();
        xRayCamera.enabled = false;
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

        if (_scoping)
        {
            playerMovementsRB.speed = originalSpeed * 0.5f;
        }
        else
        {
            playerMovementsRB.speed = originalSpeed;
        }

        //tir clique gauche normal
        if ((tapable ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && !_shooting && currentMode == FireMode.Normal)
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
        }
        /*else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToLaserMode();
        }*/
        if (playerMovementsRB.isGrounded)
        {
            Vector3 finalPosition = transform.localPosition;
            float waveslice = 0.0f;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                bobbingTimer = 0.0f;
            }
            else
            {
                waveslice = Mathf.Sin(bobbingTimer);
                bobbingTimer += bobbingSpeed;
                if (bobbingTimer > Mathf.PI * 2)
                {
                    bobbingTimer = bobbingTimer - (Mathf.PI * 2);
                }
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                finalPosition.y += translateChange;
            }

            transform.localPosition = finalPosition;
        }
        //tir clique gauche explosive

        if (currentMode == FireMode.Explosive)
        {
            if (Input.GetMouseButtonDown(0) && currentExplosiveCharges > 0)
            {
                chargeStartTime = Time.time;
                chargingEffectInstance = Instantiate(chargingEffect, chargingEffectSpawnPoint.position, chargingEffectSpawnPoint.rotation);
                chargingEffectInstance.Play();
                chargingEffectInstance.gameObject.AddComponent<VFXAutoDestroy>();
                StartCoroutine(HandleWeaponShake());
            }

            if (Input.GetMouseButtonDown(0) && currentExplosiveCharges <= 0)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Guns/ExplosiveGun/0 Charge");
            }

            if (Input.GetMouseButton(0) && currentExplosiveCharges > 0)
            {
                if (Time.time - chargeStartTime >= chargeTimeThreshold && readyToFireInstance == null)
                {
                    Debug.Log("Ready to fire particle should be instantiated");
                    readyToFireInstance = Instantiate(readyToFireEffect, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
                    readyToFireInstance.transform.SetParent(muzzleFlashSpawnPoint);
                    readyToFireInstance.Play();
                    //readyToFireInstance.gameObject.AddComponent<VFXAutoDestroy>();
                }
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
                explosiveInstance.gameObject.AddComponent<VFXAutoDestroy>();
                ExplosiveShoot();
                explosiveChargeText.text = currentExplosiveCharges + "/" + maxExplosiveCharges;

                if (explosiveTrailVFX != null)
                {
                    // Apply rotation to the particle system
                    Quaternion lookRotation = Quaternion.LookRotation(shotDirection, Vector3.up);
                    explosiveTrailVFX.transform.rotation = lookRotation;

                    // Play the particle system
                    explosiveTrailVFX.Play();
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Guns/ExplosiveGun/Shoot 2");
                }

                if (readyToFireInstance != null)
                {
                    readyToFireInstance.Stop();
                    Destroy(readyToFireInstance.gameObject);
                    readyToFireInstance = null;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (chargingEffectInstance != null)
                {
                    chargingEffectInstance.Stop();
                    Destroy(chargingEffectInstance.gameObject);
                    chargingEffectInstance = null;
                }
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

            if (smokeInstance != null)
            {
                smokeInstance.transform.position = smokeEffectSpawnPoint.position;
            }
        }


        //LASER

        /* if (Input.GetMouseButton(0) && currentMode == FireMode.Laser && canShootLaser)
         {
             StartChargingLaser();
         }*/

        if (!overheated && !_shooting)
        {
            currentHeat -= cooldownRate * Time.deltaTime;

            // Clamp pour que ce soit entre 0 et 1
            currentHeat = Mathf.Clamp01(currentHeat);
        }

        if (overheated && Time.time - cooldownStartTime >= cooldownTime)
        {
            overheated = false;
            currentHeat = 0f;
        }

        if (overheated)
        {
            if (Time.time - cooldownStartTime >= cooldownTime)
            {
                overheated = false;
                currentHeat = 0f;
                if (smokeInstance != null)
                {
                    smokeInstance.Stop();
                }
            }
            else
            {
                currentHeat -= cooldownRate * Time.deltaTime;
                currentHeat = Mathf.Clamp01(currentHeat);
                if (smokeInstance == null)
                {
                    smokeInstance = Instantiate(smokeEffect, smokeEffectSpawnPoint.position, smokeEffectSpawnPoint.rotation);
                    smokeInstance.transform.SetParent(smokeEffectSpawnPoint, true);  // Attache l'effet de fumée au spawn point
                    smokeInstance.Play();
                    smokeInstance.gameObject.AddComponent<VFXAutoDestroy>();
                }
            }
        }
        else if (!_shooting)
        {
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp01(currentHeat);
            if (smokeInstance != null)
            {
                smokeInstance.Stop();
            }
        }

        UpdateHeatUI();

        if (Input.GetMouseButton(1)) // Assumant que "Fire2" est le bouton pour viser
        {
            if (IsLookingAtTarget())
            {
                mainCamera.cullingMask = thermalViewMask;
                xRayCamera.enabled = true;
            }
            else
            {
                mainCamera.cullingMask = normalViewMask;
                xRayCamera.enabled = false;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            mainCamera.cullingMask = normalViewMask;
            xRayCamera.enabled = false;
        }
    }

    private bool IsLookingAtTarget()
    {
        if (targetObject1 == null && targetObject2 == null) return false;

        if (targetObject1 != null)
        {
            Vector3 directionToTarget1 = targetObject1.transform.position - _playerCamera.position;
            float angle1 = Vector3.Angle(_playerCamera.forward, directionToTarget1);
            if (angle1 <= angleMargin)
            {
                return true;
            }
        }

        if (targetObject2 != null)
        {
            Vector3 directionToTarget2 = targetObject2.transform.position - _playerCamera.position;
            float angle2 = Vector3.Angle(_playerCamera.forward, directionToTarget2);
            if (angle2 <= angleMargin)
            {
                return true;
            }
        }

        return false;
    }



    private void Shoot()
    {
        if (overheated)
        {
            if (Time.time - cooldownStartTime >= cooldownTime)
            {
                overheated = false;
                currentHeat = 0f;
                if (smokeInstance != null)
                {
                    smokeInstance.Stop();
                    Destroy(smokeInstance.gameObject);
                }
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

                RaycastHit hitInfo;
                if (Physics.Raycast(_playerCamera.position, shotDirection, out hitInfo, range))
                {
                    shootInstance = Instantiate(shootEffect, hitInfo.point, Quaternion.identity);
                    shootInstance.Play();
                    shootInstance.gameObject.AddComponent<VFXAutoDestroy>();
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
                overheated = true;
                cooldownStartTime = Time.time;
                if (smokeInstance == null)
                {
                    smokeInstance = Instantiate(smokeEffect, smokeEffectSpawnPoint.position, smokeEffectSpawnPoint.rotation);
                    smokeInstance.transform.SetParent(smokeEffectSpawnPoint, true);
                    smokeInstance.Play();
                    smokeInstance.gameObject.AddComponent<VFXAutoDestroy>();
                }
            }
        }

        StartCoroutine(ShootingCooldown());
    }

    private void HandleHitObject(RaycastHit hitInfo)
    {
        var rb = hitInfo.transform.GetComponent<Rigidbody>();
        var cubeHealth = hitInfo.transform.GetComponent<CubeHealth>();
        var fakeHeart = hitInfo.transform.GetComponent<FakeHeart>();
        if (rb != null)
        {
            rb.velocity += _playerCamera.forward * hitForce;
        }

        if (fakeHeart != null)
        {
            fakeHeart.TakeDamage(damage);
        }

        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(damage, false); // Passe false pour isExplosiveDamage

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
            CubeHitInstance = Instantiate(CubeHit, hitInfo.transform.position, hitInfo.transform.rotation);
            CubeHitInstance.Play();
            CubeHitInstance.gameObject.AddComponent<VFXAutoDestroy>();
            Destroy(hitInfo.transform.gameObject);
        }
    }


    private IEnumerator ShootingCooldown()
    {
        _shooting = true;
        yield return new WaitForSeconds(GetAdjustedFireRate());
        _shooting = false;
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

    private float GetAdjustedFireRate()
    {
        float baseFireRate = 1f / shotsPerSecond;
        float adjustedFireRate = baseFireRate / (1f + heatEffectMultiplier * currentHeat);
        return adjustedFireRate;
    }

   


    public void Pickup(Transform weaponHolder, Transform playerCamera)
    {
        if (playerMovementsRB != null)
        {
            originalSpeed = playerMovementsRB.speed;
        }

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
        _scoping = false;
        weaponUIPanel.SetActive(true);
    }



    #region SWITCH TYPE ARMES
    public void SwitchFireMode()
    {
        currentMode = (currentMode == FireMode.Normal) ? FireMode.Explosive : FireMode.Normal;
    }

    private void SwitchToNormalMode()
    {
        currentMode = FireMode.Normal;
        ChangeMaterial(currentMode);
    }

    private void SwitchToExplosiveMode()
    {
        currentMode = FireMode.Explosive;
        ChangeMaterial(currentMode);
    }

    /*private void SwitchToLaserMode()
    {
        currentMode = FireMode.Laser;
        ChangeMaterial(currentMode);
    }*/

    private void ChangeMaterial(FireMode mode)
    {
        Material newMaterial = null;

        switch (mode)
        {
            case FireMode.Normal:
                newMaterial = normalModeMaterial;
                break;
            case FireMode.Explosive:
                newMaterial = explosiveModeMaterial;
                break;
           /* case FireMode.Laser:
                newMaterial = laserModeMaterial;
                break;*/
        }

        if (weaponModel != null && newMaterial != null)
        {
            var renderer = weaponModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }

    #endregion

    #region MODE EXPLOSIVE
    private void ExplosiveShoot()
    {
        if (currentExplosiveCharges > 0)
        {
            StartCoroutine(HandleExplosiveRecoil(kickbackTimeExplosive, kickbackForceExplosive)); // Adjust the duration and force as needed

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, explosiveRange))
            {
                explosionInstance = Instantiate(explosionEffect, hit.point, Quaternion.identity);
                explosionInstance.Play();
                explosionInstance.gameObject.AddComponent<VFXAutoDestroy>();

                Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
                foreach (Collider hitCollider in colliders)
                {
                    CubeHealth cubeHealth = hitCollider.GetComponent<CubeHealth>();
                    if (cubeHealth != null)
                    {
                        cubeHealth.TakeDamage(explosiveDamage, true);
                    }

                    if (hitCollider.transform.CompareTag("DestroyableBlock"))
                    {
                        CubeHitInstance = Instantiate(CubeHit, hitCollider.transform.position, hitCollider.transform.rotation);
                        CubeHitInstance.Play();
                        CubeHitInstance.gameObject.AddComponent<VFXAutoDestroy>();
                        Destroy(hitCollider.transform.gameObject);
                    }
                }


            }

            currentExplosiveCharges--;
            explosiveChargeText.text = currentExplosiveCharges + "/" + maxExplosiveCharges;
        }
    }



    public void GainExplosiveCharge()
    {
        if (currentExplosiveCharges < maxExplosiveCharges)
        {
            currentExplosiveCharges++;
            explosiveChargeText.text = currentExplosiveCharges + "/" + maxExplosiveCharges;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Collectibles/Collected");
            

            // Appeler la méthode pour afficher l'image
            FindObjectOfType<WeaponUIManager>().ShowExplosiveChargeGain();
        }
    }

    private IEnumerator HandleExplosiveRecoil(float duration, float force)
    {
        Vector3 initialPosition = transform.localPosition;
        Vector3 recoilPosition = initialPosition - new Vector3(0, 0, force);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(recoilPosition, initialPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
    }

    private IEnumerator HandleWeaponShake()
    {
        while (Input.GetMouseButton(0))
        {
            float chargeDuration = Time.time - chargeStartTime;
            float normalizedCharge = chargeDuration / chargeTimeThreshold;
            float shakeIntensity = shakeCurve.Evaluate(normalizedCharge);

            ApplyShake(shakeIntensity);

            yield return null;
        }
    }

    private void ApplyShake(float intensity)
    {
        float shakeOffsetX = Mathf.PerlinNoise(Time.time * 25f, 0f) * 2 - 1;
        float shakeOffsetY = Mathf.PerlinNoise(0f, Time.time * 25f) * 2 - 1;
        float shakeOffsetZ = Mathf.PerlinNoise(Time.time * 25f, Time.time * 25f) * 2 - 1;

        Vector3 shakeOffset = new Vector3(shakeOffsetX, shakeOffsetY, shakeOffsetZ) * intensity;
        transform.localPosition += shakeOffset;
    }



#endregion

/* #region MODE LASER

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
         Vector3 laserEnd = transform.position + transform.forward;
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
     WeaponUIManager weaponUIManager = FindObjectOfType<WeaponUIManager>();
     weaponUIManager.DisplayLaserCooldownText(laserCooldown);
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
*/
public bool Scoping => _scoping;
    
}