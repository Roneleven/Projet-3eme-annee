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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

        if (jetpack.action.IsPressed())
        {
            UseJetpack();
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
            rb.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);

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
