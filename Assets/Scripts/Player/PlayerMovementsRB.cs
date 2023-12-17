using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetPlayerInput();

        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 move = transform.right * movementX + transform.forward * movementY;

        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        if (transform.position.y < -100)
        {
            transform.position = respawnPoint.position;
        }
    }

    // Input handling methods
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void OnJump(InputValue jumpValue)
    {
        if (isGrounded)
        {
            // Code pour gérer le saut
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
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
}