using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    public float tiltAngle; // Maximum tilt angle
    public float smooth;    // Smoothing factor for tilting

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.localRotation; // Initialize with current local rotation
    }

    void Update()
    {
        float tilt = Input.GetAxis("Horizontal") * tiltAngle; // Calculate tilt based on player input
        Quaternion tiltRotation = Quaternion.Euler(0, 0, -tilt); // Apply tilt on Z-axis

        // Smoothly interpolate to the target rotation, in local space
        targetRotation = Quaternion.Slerp(targetRotation, tiltRotation, Time.deltaTime * smooth);
        transform.localRotation = targetRotation;
    }
}
