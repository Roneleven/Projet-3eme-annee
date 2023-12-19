using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementsRB : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform respawnPoint;
    public float respawnPositionY;

    public float jetpackForce = 10f;
    public float jetpackChargeRate = 1f;
    public float maxJetpackCharge = 100f;
    public InputActionReference jetpack;

    public Image jetpackChargeImage;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    private float jetpackCharge;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;
    }

    private void Update()
    {
        GetPlayerInput();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

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
        // Gestion de la physique dans FixedUpdate
        // Ajout de force du jetpack dans FixedUpdate
        if (jetpack.action.IsPressed())
        {
            UseJetpack();
        }
    }

    private void UseJetpack()
    {
        if (jetpackCharge > 0)
        {
            rb.AddForce(Vector3.up * jetpackForce * Time.deltaTime, ForceMode.Force);
            jetpackCharge = Mathf.Max(jetpackCharge - Time.deltaTime, 0);
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
