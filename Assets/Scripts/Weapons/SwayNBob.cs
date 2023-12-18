using UnityEngine;

public class SwayNBobScript : MonoBehaviour
{
    public PlayerMovements playerMovement;

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
    private float curveSin { get => Mathf.Sin(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    private Vector3 bobEulerRotation;

    public Vector2 walkInput;
    public Vector2 lookInput;

    private void Update()
    {
        walkInput = playerMovement.GetMovementInput();
        lookInput = playerMovement.GetLookInput();
        bool isGrounded = playerMovement.IsGrounded();

        Sway(lookInput);
        Debug.Log(lookInput);
        Bob(walkInput, isGrounded);
        CompositePositionRotation();
    }

    private void Sway(Vector2 lookInput)
    {
        Vector3 invertLook = new Vector3(-lookInput.x, -lookInput.y, 0) * step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
        SwayRotation(invertLook);
    }

    private void SwayRotation(Vector3 invertLook)
    {
        Vector3 rotationDelta = new Vector3(invertLook.y, invertLook.x, 0) * rotationStep;
        rotationDelta = new Vector3(
            Mathf.Clamp(rotationDelta.x, -maxRotationStep, maxRotationStep),
            Mathf.Clamp(rotationDelta.y, -maxRotationStep, maxRotationStep),
            Mathf.Clamp(rotationDelta.z, -maxRotationStep, maxRotationStep)
        );

        swayEulerRot = rotationDelta;
    }

    private void Bob(Vector2 walkInput, bool isGrounded)
    {
        if (isGrounded)
        {
            speedCurve += Time.deltaTime * walkInput.magnitude * bobExaggeration;
            bobPosition = new Vector3(0, Mathf.Sin(speedCurve) * bobLimit.y, 0);
        }
        else
        {
            bobPosition = Vector3.zero;
        }
        BobRotation(walkInput, isGrounded);
    }

    private void BobRotation(Vector2 walkInput, bool isGrounded)
    {
        if (isGrounded && walkInput != Vector2.zero)
        {
            bobEulerRotation.x = multiplier.x * Mathf.Sin(2 * speedCurve);
            bobEulerRotation.y = multiplier.y * curveCos;
            bobEulerRotation.z = multiplier.z * curveCos * walkInput.x;
        }
        else
        {
            bobEulerRotation = Vector3.zero;
        }
    }

    private void CompositePositionRotation()
    {
        Vector3 compositePosition = swayPos + bobPosition + new Vector3(0.5f, -1f, 1);
        transform.localPosition = Vector3.Lerp(transform.localPosition, compositePosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot + bobEulerRotation), Time.deltaTime * smoothRot);
    }
}
