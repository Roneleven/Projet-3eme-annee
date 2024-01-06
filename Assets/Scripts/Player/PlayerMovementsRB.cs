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
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
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

    [Header("Jetpack Shake Settings")]
    public float shakeDuration;
    public float shakeForce;

    [Header("Jetpack UI Settings")]
    public Image jetpackChargeImage;

    private Rigidbody rb;
    public float movementX;
    public float movementY;
    public bool isGrounded;
    public Vector3 Velocity => rb.velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;
    }

    private void Update()
    {
        GetPlayerInput();

        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }

        if (jetpack.action.IsPressed())
        {
            UseJetpack();
        }

        UpdateJetpackChargeUI();

        if (transform.position.y < respawnPositionY)
        {
            Respawn();
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

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

        if (jetpack.action.IsPressed())
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
            CameraShake.Shake(shakeDuration, shakeForce);
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
