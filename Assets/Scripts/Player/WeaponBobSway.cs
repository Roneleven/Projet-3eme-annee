using UnityEngine;

public class SwayNBobScript : MonoBehaviour
{
    public PlayerMovementsRB playerMovement; // Reference to PlayerMovementsRB script

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    private Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    private Vector3 swayEulerRot; 

    public float smooth = 10f;
    private float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    private float curveSin => Mathf.Sin(speedCurve);
    private float curveCos => Mathf.Cos(speedCurve);

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    private Vector3 bobEulerRotation;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector2 walkInput;

    void Start()
    {
        // Capture the initial position and rotation
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    void Sway()
    {
        // Using velocity for sway calculations
        Vector3 invertLook = new Vector3(walkInput.y, -walkInput.x, 0) * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        // Using velocity for rotation calculations
        Vector2 invertLook = new Vector2(walkInput.y, -walkInput.x) * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        // Apply sway and bob relative to the initial position and rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (playerMovement.isGrounded ? (walkInput.x + walkInput.y) * bobExaggeration : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (playerMovement.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (walkInput.y * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * Mathf.Sin(2 * speedCurve) : multiplier.x * Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }

    void LateUpdate()
    {
        // Update walkInput based on PlayerMovementsRB's velocity
        Vector3 localVelocity = transform.InverseTransformDirection(playerMovement.Velocity);
        walkInput = new Vector2(localVelocity.x, -localVelocity.z);
    }
}
