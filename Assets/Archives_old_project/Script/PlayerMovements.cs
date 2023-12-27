using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController controller;
    public float speed;
    public float gravity = -9.81f; // Base gravity
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private float movementX;
    private float movementY;
    public float bumperJumpHeight;
    public Transform respawnPoint;

    Vector3 velocity;
    bool isGrounded;

    private void Update()
{
    GetPlayerInput();

    // Check if grounded
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    // Apply gravity and movement
    if (isGrounded && velocity.y < 0)
    {
        velocity.y = -2f;
    }

    Vector3 move = transform.right * movementX + transform.forward * movementY;

    controller.Move(move.normalized * speed * Time.deltaTime);

    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);

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
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Bumper"))
        {
            float originalJumpHeight = jumpHeight;
            jumpHeight = bumperJumpHeight;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpHeight = originalJumpHeight;
        }
    }

    private float xInput;
    private float zInput;

    void GetPlayerInput()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
    }
}