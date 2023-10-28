using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed;
    public float gravity = -9.81f; // Gravité de base
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private float movementX;
    private float movementY;
    public int speedIncrement = 0; // 0, 1, 2, 3
    public int gravityIncrement = 0; // 0, 1, 2, 3
    public int incrementalValue = 1; // Valeur d'incrémentation

    Vector3 velocity;
    bool isGrounded;

    // Tableaux de valeurs à modifier
    public float[] speedValues = { 10f, 15f, 20f, 25f, 30f };
    public float[] gravityValues = { -9.81f, -9.5f, -9.0f, -8.5f, -8.0f };

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * movementX + transform.forward * movementY;

        float modifiedSpeed = speedValues[speedIncrement];
        float modifiedGravity = gravityValues[gravityIncrement];

        controller.Move(move.normalized * modifiedSpeed * Time.deltaTime);

        velocity.y += modifiedGravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

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

    public void IncrementSpeed()
    {
        if (speedIncrement < 4)
        {
            speedIncrement++;
        }
    }

    public void DecrementSpeed()
    {
        if (speedIncrement > 0) 
        {
            speedIncrement--;
        }
    }

    public void IncrementGravity()
    {
        if (gravityIncrement < 4)
        {
            gravityIncrement++;
        }
    }

    public void DecrementGravity()
    {
        if (gravityIncrement > 0)
        {
            gravityIncrement--;
        }
    }

    private void OnSpeedIncrement()
    {
        IncrementSpeed();
    }

    private void OnSpeedDecrement()
    {
        DecrementSpeed();
    }

    private void OnGravityIncrement()
    {
        IncrementGravity();
    }

    private void OnGravityDecrement()
    {
        DecrementGravity();
    }
}