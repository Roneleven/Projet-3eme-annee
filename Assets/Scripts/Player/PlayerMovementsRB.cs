using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementsRB : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed;
    private float initialSpeed;
    public float currentSpeed;
    public float maxPossibleSpeed;
    public float fallAccelerationForce;
    public Transform groundCheck;
    public float groundDistance;
    public List<LayerMask> groundMasks;
    public bool canMove = true;

    public CameraController cameraController;

    [Header("Respawn Settings")]
    /*public Transform respawnPoint;
    public float respawnPositionY;*/

    [Header("Jetpack Settings")]
    public float jetpackForce = 10f;
    public float jetpackForceMultiplier;
    private float initialJetpackForce;
    public float jetpackChargeRate = 1f;
    public float maxJetpackCharge = 100f;
    public float jetpackCharge;
    public InputActionReference jetpack;
    public ParticleSystem jetpackEffect;
    private bool isJetpackPressed = false;
    private bool isJetpackEmptySoundPlayed = false;
    private bool isGroundedSoundPlayed = false;
    private bool isPlanningSoundPlayed = false;
    private FMOD.Studio.EventInstance planning;
    private bool isPlanningPlaying = false;

    [Header("Jetpack Shake Settings")]
    public float shakeDuration;
    public float shakeForce;

    [Header("Jetpack UI Settings")]
    public Image jetpackChargeImage;

    public Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    private FMOD.Studio.EventInstance jetUse;
    private bool isJetUsePlaying = false;
    public HeartSpawner heartSpawner;
    public float glideForce = 5f; // Valeur par défaut de la force de planeur

    Vector3 moveDirection;
    public Transform orientation;
    public float airMultiplier;
    private Vector3 velocityRef = Vector3.zero;
    public float smoothTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;
        jetUse = FMODUnity.RuntimeManager.CreateInstance("event:/Character/Locomotion/JetpackUsing");
        planning = FMODUnity.RuntimeManager.CreateInstance("event:/Character/Locomotion/Planning");
        initialJetpackForce = jetpackForce; // Store the initial jetpack force
        initialSpeed = speed;
    }

    private void Update()
    {
        SpeedControl();
        GetPlayerInput();

        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }
        if (jetpack.action.IsPressed() && jetpackCharge > 0)
        {
            if (!isJetUsePlaying)
            {
                jetUse.start();
                isJetUsePlaying = true;
            }
            UseJetpack();
        }
        else
        {
            if (isJetUsePlaying)
            {
                jetUse.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isJetUsePlaying = false;
                jetpackEffect.Stop();
            }
            jetpackForce = initialJetpackForce; // Reset jetpack force when input is released
            speed = initialSpeed;
        }

        UpdateJetpackChargeUI();

        /*if (transform.position.y < respawnPositionY)
        {
            Respawn();
        }*/

        if (isGrounded)
        {
            jetpackEffect.Stop();
        }

        if (jetpackCharge <= 0 && !isJetpackEmptySoundPlayed)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Locomotion/JetpackEmpty");
            isJetpackEmptySoundPlayed = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded && rb.velocity.magnitude > maxPossibleSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxPossibleSpeed;
        }

        isGrounded = CheckGround();

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * fallAccelerationForce, ForceMode.Acceleration);
            isGroundedSoundPlayed = false;
        }

        if (jetUse.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            jetUse.getPlaybackState(out state);
            isJetUsePlaying = state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        }

        if (!isGrounded && jetpackCharge <= 0 && jetpack.action.IsPressed())
        {
            rb.useGravity = false;
            rb.AddForce(Vector3.down * glideForce, ForceMode.Force); // Force vers le bas sans gravité
            rb.AddForce(Vector3.up * fallAccelerationForce, ForceMode.Acceleration);
            isPlanningSoundPlayed = true;
        }
        else
        {
            rb.useGravity = true;
            isPlanningSoundPlayed = false;

            if (jetpackCharge > 0 && jetpack.action.IsPressed())
            {
                UseJetpack();
            }
        }

        if (isPlanningSoundPlayed && !isPlanningPlaying)
        {
            planning.start();
            isPlanningPlaying = true;
        }
        else if (!isPlanningSoundPlayed && isPlanningPlaying)
        {
            planning.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isPlanningPlaying = false;
        }

        MovePlayer();

        currentSpeed = rb.velocity.magnitude;

        if (isGrounded && !isGroundedSoundPlayed)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Locomotion/Grounded");
            isGroundedSoundPlayed = true;
        }
    }

    private void MovePlayer()
    {
        if (!canMove)
        {
            return;
        }

        Vector3 move = new Vector3(movementX, 0f, movementY);
        Vector3 localMove = transform.TransformDirection(move);

        Vector3 targetVelocity = localMove * speed;
        if (isGrounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocityRef, smoothTime);
        }
        else
        {
            rb.AddForce(targetVelocity - rb.velocity, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        if (flatVel.magnitude > maxPossibleSpeed)
        {
            Vector3 limitedFlatVel = flatVel.normalized * maxPossibleSpeed;
            rb.velocity = new Vector3(limitedFlatVel.x, rb.velocity.y, limitedFlatVel.z);
        }
    }

    private bool CheckGround()
    {
        foreach (LayerMask groundMask in groundMasks)
        {
            if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
            {
                return true;
            }
        }
        return false;
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void UseJetpack()
    {
        if (jetpackCharge > 0)
        {
            jetpackForce += Time.deltaTime * jetpackForceMultiplier; // Increase jetpack force over time
            speed +=  Time.deltaTime * jetpackForceMultiplier;
            rb.velocity = new Vector3(rb.velocity.x, jetpackForce, rb.velocity.z);
            jetpackCharge = Mathf.Max(jetpackCharge - Time.deltaTime, 0);

            if (!jetpackEffect.isPlaying)
            {
                jetpackEffect.Play();
            }

            CameraShake.Shake(shakeDuration, shakeForce);
            isJetpackEmptySoundPlayed = false;
        }
        else
        {
            jetpackEffect.Stop();
        }
    }

    private void GetPlayerInput()
    {
        movementX = Input.GetAxis("Horizontal");
        movementY = Input.GetAxis("Vertical");

        isJetpackPressed = jetpack.action.triggered;
    }

    private void UpdateJetpackChargeUI()
    {
        if (jetpackChargeImage != null)
        {
            jetpackChargeImage.fillAmount = jetpackCharge / maxJetpackCharge;
        }
    }

    private void OnStopTimer()
    {
        heartSpawner.timer += 60;
    }

    private void OnResetTimer()
    {
        heartSpawner.timer = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            if (cameraController != null)
            {
                cameraController.DropCamera();
            }
        }
    }
}
