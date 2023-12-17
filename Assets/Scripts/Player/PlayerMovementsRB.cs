using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);


        // Respawn logic
        if (transform.position.y < respawnPositionY)
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
