using UnityEngine;

public class WeaponBobSway : MonoBehaviour
{
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float swayAmount = 0.2f;
    public float maxSwayAngle = 5.0f;
    public Camera playerCamera;

    private float timer = 0.0f;
    private Vector3 restPosition;
    private Quaternion restRotation;
    private Vector2 lastMousePosition;

    void Start()
    {
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
        lastMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 localPosition = transform.localPosition;
        Quaternion localRotation = transform.localRotation;

        // Bobbing
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer -= (Mathf.PI * 2);
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange *= totalAxes;
            localPosition.y = restPosition.y + translateChange;
        }
        else
        {
            localPosition.y = restPosition.y;
        }

        // Position Sway
        float factorX = -Input.GetAxis("Mouse X") * swayAmount;
        float factorY = -Input.GetAxis("Mouse Y") * swayAmount;
        Vector3 finalPosition = new Vector3(localPosition.x + factorX, localPosition.y, localPosition.z + factorY);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * swayAmount);

        // Rotation Sway based on camera movement
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 mouseDeltaDifference = mouseDelta - lastMousePosition;
        lastMousePosition = mouseDelta;

        float tiltAroundZ = mouseDeltaDifference.x * maxSwayAngle;
        float tiltAroundX = mouseDeltaDifference.y * maxSwayAngle;

        Quaternion finalRotation = Quaternion.Euler(-tiltAroundX, 0, tiltAroundZ);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, restRotation * finalRotation, Time.deltaTime * swayAmount);
    }
}