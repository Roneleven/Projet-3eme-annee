using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementsRB : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed;
    public Transform groundCheck;
    public float groundDistance;
    public List<LayerMask> groundMasks;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnPositionY;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;
        jetUse = FMODUnity.RuntimeManager.CreateInstance("event:/Character/Locomotion/JetUse");
    }

    private void Update()
    {
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

        if (transform.position.y < respawnPositionY)
        {
            Respawn();
        }

        if (isGrounded)
        {
            jetpackEffect.Stop();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = false;

        foreach (LayerMask groundMask in groundMasks)
        {
            if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
            {
                isGrounded = true;
                break;
            }
        }

        if (jetUse.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            jetUse.getPlaybackState(out state);
            isJetUsePlaying = state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        }


        Vector3 move = new Vector3(movementX, 0f, movementY);
        move = transform.TransformDirection(move);
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);  // Appliquez la vélocité au Rigidbody tout en conservant la composante Y


        if (jetpack.action.IsPressed() && jetpackCharge > 0)
        {
            UseJetpack();
        }

        if (isGrounded && (Mathf.Abs(movementX) > 0 || Mathf.Abs(movementY) > 0))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Locomotion/Footsteps");
        }
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

    private void Respawn()
    {
        transform.position = respawnPoint.position;
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

    private void OnSpeedIncrement()
    {
        speed += 20;
    }

    private void OnSpeedDecrement()
    {
        speed = 17;
    }
}
