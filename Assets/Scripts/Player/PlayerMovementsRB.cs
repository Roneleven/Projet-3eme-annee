using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementsRB : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float bumperJumpHeight;
    public Transform respawnPoint;

    public float jetpackForce = 10f;
    public float jetpackChargeRate = 1f;
    public float maxJetpackCharge = 100f;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    public float jetpackCharge;
    public InputActionReference jetpack;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;
    }

    private void Update()
    {
        GetPlayerInput();

        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Recharge du jetpack au sol
        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        // Utilisation du jetpack
        if (!isGrounded && jetpackCharge > 0)
        {
            //OnJump();
        }

        // Respawn logic
        if (transform.position.y < -100)
        {
            Respawn();
        }

        if (jetpack.action.IsPressed()){
            UseJetpack();
           }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    /*private void OnJump(InputValue jetpackValue)
    {
        float jetpackInput = jetpackValue.Get<float>();
        if (jetpackInput > 0 && jetpackCharge > 0)
        {
            UseJetpack();
        }
        else
        {

        }
    }*/

    private void UseJetpack()
    {
        rb.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);
        jetpackCharge = Mathf.Max(jetpackCharge - Time.deltaTime, 0);
    }

    /*void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Bumper"))
        {
            float originalJumpHeight = jumpHeight;
            jumpHeight = bumperJumpHeight;
            // Code pour gérer le saut depuis le bumper
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            jumpHeight = originalJumpHeight;
        }
    }*/

    private void GetPlayerInput()
    {
        movementX = Input.GetAxis("Horizontal");
        movementY = Input.GetAxis("Vertical");
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
    }
}
