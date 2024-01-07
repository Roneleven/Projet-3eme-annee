using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxAmount = 0.03f;
    public float smoothAmount = 6f;
    public float rotationMultiplier = 5f; // Multiplier for rotation sway

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate sway based on mouse input
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        // Calculate new position and rotation
        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        Quaternion finalRotation = new Quaternion(movementY * rotationMultiplier, movementX * rotationMultiplier, 0, 1);

        // Interpolate to new position and rotation smoothly
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmount);
    }
}
