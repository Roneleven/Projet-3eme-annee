using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementRb : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 moveInput;
    bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            // Reset velocity when grounded
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            moveInput = transform.right * x + transform.forward * z;

            if (Input.GetButtonDown("Jump"))
            {
                // Apply an upward force for jumping
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        // Apply movement using Rigidbody
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}