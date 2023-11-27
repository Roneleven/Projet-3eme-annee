//Lien pour les collectibles https://youtu.be/jJonSISrTqQ?list=PLdPQ93duD7PCkWpTo4ElrWlyx6S9bF18V
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensivity = 3f;
    [SerializeField] float walkingSpeed = 5f;
    [SerializeField] float flyingSpeed = 10f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] Transform cameraTransform;

    public bool isGrounded => controller.isGrounded;

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    internal float movementSpeedMultiplier;

    public State state;

    public enum State
    {
        Walking,
        Flying
    }

    CharacterController controller;
    internal Vector3 velocity;
    Vector2 look;

    bool wasGrounded;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction flyUpDownAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        flyUpDownAction = playerInput.actions["flyUpDown"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        movementSpeedMultiplier = 1f;
        switch (state)
        {
            case State.Walking:
                UpdateGround();
                UpdateGravity();
                UpdateMovement();
                UpdateLook();
                break;
            case State.Flying:
                UpdateMovementFlying();
                UpdateLook();
                break;
        }
    }

    void UpdateGround()
    {
        if(wasGrounded != isGrounded)
        {
            OnGroundStateChange?.Invoke(isGrounded);
            wasGrounded = isGrounded;
        }
    }

    void UpdateGravity() 
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    Vector3 GetMovementInput(float speed, bool horizontal = true)
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var flyUpDownInput = flyUpDownAction.ReadValue<float>();
        var input = new Vector3();
        var referenceTransform = horizontal ? transform : cameraTransform;
        input += referenceTransform.forward * moveInput.y;
        input += referenceTransform.right * moveInput.x;
        if (!horizontal)
        {
            input += transform.up * flyUpDownInput;
        }
        input = Vector3.ClampMagnitude(input, 1f);

        input *= speed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement()
    {
        OnBeforeMove?.Invoke();

        var input = GetMovementInput(walkingSpeed);

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateMovementFlying()
        {
            var input = GetMovementInput(flyingSpeed, false);

            var factor = acceleration * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, input, factor);

            controller.Move(velocity * Time.deltaTime);
        }
    

    void UpdateLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensivity;
        look.y += lookInput.y * mouseSensivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler (-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);

    }

    void OnToggleFlying()
    {
        state = state == State.Flying ? State.Walking : State.Flying;
    }
}
