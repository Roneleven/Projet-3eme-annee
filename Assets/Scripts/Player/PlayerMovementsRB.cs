using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementsRB : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed;
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnPositionY;

    [Header("Jetpack Settings")]
    public float jetpackForce = 10f;
    public float jetpackChargeRate = 1f;
    public float maxJetpackCharge = 100f;
    public float jetpackCharge;
    public InputActionReference jetpack;

    [Header("Jetpack UI Settings")]
    public Image jetpackChargeImage;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    private FMOD.Studio.EventInstance jetUse;
    private bool isJetUsePlaying = false;

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
            }
        }

        UpdateJetpackChargeUI();

        if (transform.position.y < respawnPositionY)
        {
            Respawn();
        }
    }

    private void FixedUpdate()
    {
        if (jetUse.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            jetUse.getPlaybackState(out state);
            isJetUsePlaying = state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

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
            rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);

            jetpackCharge = Mathf.Max(jetpackCharge - Time.fixedDeltaTime, 0);
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
}
