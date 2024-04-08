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
    public float currentSpeed;
    public float fallAccelerationForce;
    public Transform groundCheck;
    public float groundDistance;
    public List<LayerMask> groundMasks;

    [Header("Respawn Settings")]
    /*public Transform respawnPoint;
    public float respawnPositionY;*/

    [Header("Jetpack Settings")]
    public float jetpackForce = 10f;
    public float jetpackChargeRate = 1f;
    public float maxJetpackCharge = 100f;
    public float jetpackCharge;
    public InputActionReference jetpack;
    public ParticleSystem jetpackEffect;

    [Header("Jetpack Shake Settings")]
    public float shakeDuration;
    public float shakeForce;

    [Header("Jetpack UI Settings")]
    public Image jetpackChargeImage;

    private Rigidbody rb;
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
        jetUse = FMODUnity.RuntimeManager.CreateInstance("event:/Character/Locomotion/JetUse");
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
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * fallAccelerationForce, ForceMode.Acceleration);
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
            rb.AddForce(Vector3.down * glideForce, ForceMode.Force); //force vers le bas sans gravité
            rb.AddForce(Vector3.up * fallAccelerationForce, ForceMode.Acceleration);
            //Physics.gravity = new Vector3(0f, glideForce, 0f); //utilisation de la gravité 
        }
        else
        {
            rb.useGravity = true;
            //Physics.gravity = new Vector3(0f, -9.81f, 0f);

            // Gérer l'utilisation du jetpack
            if (jetpackCharge > 0 && jetpack.action.IsPressed())
            {
                UseJetpack();
            }
        }

        if (isGrounded && (Mathf.Abs(movementX) > 0 || Mathf.Abs(movementY) > 0))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Locomotion/Footsteps");
        }

        MovePlayer();

        currentSpeed = rb.velocity.magnitude;
    }

    /*private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * movementY + orientation.right * movementX;

        // on ground
        if (isGrounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

        // in air
        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);
    }*/

    private void MovePlayer()
    {
        Vector3 move = new Vector3(movementX, 0f, movementY).normalized;
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

        // limit velocity if needed
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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
            rb.velocity = new Vector3(rb.velocity.x, jetpackForce, rb.velocity.z);
            jetpackCharge = Mathf.Max(jetpackCharge - Time.deltaTime, 0);

            if (!jetpackEffect.isPlaying)
            {
                jetpackEffect.Play();
            }

            CameraShake.Shake(shakeDuration, shakeForce);
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
    }

    /*private void Respawn()
    {
        transform.position = respawnPoint.position;
    }*/

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

}
