using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("Jetpack UI Settings")]
    public Image jetpackChargeImage;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.5f;
    public AnimationCurve shakeAccelerationCurve;
    public AnimationCurve shakeDecelerationCurve;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    public Camera mainCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jetpackCharge = maxJetpackCharge;

    }

    private void Update()
    {
        GetPlayerInput();

        if (isGrounded)
        {
            jetpackCharge = Mathf.Min(jetpackCharge + jetpackChargeRate * Time.deltaTime, maxJetpackCharge);
        }

        if (jetpack.action.IsPressed())
        {
            StartCoroutine(ShakeCamera());
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

        if (isGrounded && (Mathf.Abs(movementX) > 0 || Mathf.Abs(movementY) > 0))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Locomotion/Footsteps");
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void UseJetpack()
    {
        if (jetpackCharge > 0)
        {
            rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);
            jetpackCharge = Mathf.Max(jetpackCharge - Time.fixedDeltaTime, 0);
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

    private IEnumerator ShakeCamera()
    {
        if (mainCamera == null)
        {
            yield break; // Sortir de la coroutine si la caméra n'est pas trouvée
        }

        float elapsedTime = 0f;
        Vector3 originalPosition = mainCamera.transform.localPosition;

        while (elapsedTime < shakeDuration)
        {
            float shakeAcceleration = shakeAccelerationCurve.Evaluate(elapsedTime / shakeDuration);
            mainCamera.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAcceleration;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
}
