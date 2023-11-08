using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed;
    public float gravity = -9.81f; // Base gravity
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask zone1Mask;
    public LayerMask zone2Mask;
    public float gravityZone1;
    public float speedZone1;
    private float movementX;
    private float movementY;
    public float bumperJumpHeight;


    Vector3 velocity;
    bool isGrounded;

    // Zone variables
    public bool inZone1 = false;
    public bool inZone2 = false;

    private float defaultSpeed;
    private float defaultGravity;

    void Start()
    {
        defaultSpeed = speed;
        defaultGravity = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in zone 1
        if (Physics.CheckSphere(groundCheck.position, groundDistance, zone1Mask))
        {
            inZone1 = true;
        }
        else
        {
            inZone1 = false;
        }

        // Check if player is in zone 2
        if (Physics.CheckSphere(groundCheck.position, groundDistance, zone2Mask))
        {
            inZone2 = true;
        }
        else
        {
            inZone2 = false;
        }

        // Apply zone effects
        if (inZone1)
        {
            gravity = gravityZone1; 
            speed = speedZone1; 
        }
        else if (inZone2)
        {
            jumpHeight = 0f;
            gravity = defaultGravity; 
            speed = defaultSpeed; 
        }
        else
        {
            gravity = defaultGravity; 
            speed = defaultSpeed; 
            jumpHeight = 2f; 
        }

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
        if (isGrounded && !inZone2)
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

}