using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController controller;
    public float speed;
    public float gravity = -9.81f;
    public float jumpHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float bumperJumpHeight;
    public Transform respawnPoint;

    private Vector3 velocity;
    private bool isGrounded;
    private Vector2 movementInput;
    public Vector2 lookInput;

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        controller.Move(move.normalized * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (transform.position.y < -100)
        {
            transform.position = respawnPoint.position;
        }
        Debug.Log("Look Input: " + lookInput);
    }

    public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    public void OnJump(InputValue jumpValue)
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
            velocity.y = Mathf.Sqrt(bumperJumpHeight * -2f * gravity);
        }
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }


    // Public methods to provide movement and look data
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }

    public Vector2 GetLookInput()
    {
        return lookInput;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
