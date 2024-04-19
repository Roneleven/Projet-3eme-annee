using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerMovementsRB playerMovements;
    public MouseLook mouseLook;

    public GameObject targetToFollow;
    public float baseSpeed = 5f;
    public float distanceThreshold = 0.5f;
    public float dropCameraDelay = 0.5f;
    public float speedMultiplier = 1.2f;
    public float baseFOV = 60f;
    public float maxFOV = 120f;

    public GameObject mainCameraObject;
    public GameObject invincibility;
    public Transform playerCamera;

    private Camera mainCamera;
    private bool isFollowing = false;
    private float dropCameraTime = 0f;
    private Quaternion initialRotation;
    private float rotationLerpDuration = 1f; // Durée en secondes pour le mouvement fluide vers la rotation nulle
    private float rotationLerpStartTime;
    private float fovLerpDuration = 3f; // Durée en secondes pour le mouvement fluide du FOV de base au FOV maximum
    private float fovLerpStartTime;

    void Start()
    {
        if (mainCameraObject != null)
        {
            mainCamera = mainCameraObject.GetComponent<Camera>();
        }
    }

    public void DropCamera()
    {
        if (transform.parent != null)
        {
            if (invincibility != null)
            {
                invincibility.SetActive(true);
            }

            playerMovements.canMove = false;
            transform.parent = null;
            isFollowing = true;
            dropCameraTime = Time.time;
        }
    }

    void Update()
    {
        if (isFollowing && targetToFollow != null)
        {
            float distance = Vector3.Distance(transform.position, targetToFollow.transform.position);
            float mouseX = Input.GetAxis("Mouse X") * mouseLook.mouseSensivity * Time.deltaTime;

            playerCamera.Rotate(Vector3.up * mouseX);

            if (Time.time >= dropCameraTime + dropCameraDelay && distance < distanceThreshold)
            {
                transform.parent = targetToFollow.transform;
                isFollowing = false;

                // Stocker la rotation initiale et le temps de début de l'interpolation
                initialRotation = transform.localRotation;
                rotationLerpStartTime = Time.time;

                // Définir le temps de début de l'interpolation pour le FOV
                fovLerpStartTime = Time.time;

                if (invincibility != null)
                {
                    invincibility.SetActive(false);
                }
                playerMovements.canMove = true;

            }
            else
            {
                float currentSpeed = baseSpeed * Mathf.Pow(speedMultiplier, Time.time - dropCameraTime);

                Vector3 direction = targetToFollow.transform.position - transform.position;
                direction.Normalize();
                transform.position += direction * currentSpeed * Time.deltaTime;

                // Interpolation continue du FOV
                float t = (Time.time - fovLerpStartTime) / fovLerpDuration;
                float currentFOV = Mathf.Lerp(baseFOV, maxFOV, t);
                currentFOV = Mathf.Clamp(currentFOV, baseFOV, maxFOV);
                if (mainCamera != null)
                {
                    mainCamera.fieldOfView = currentFOV;
                }
            }
        }
        
        // Interpolation de la rotation vers la rotation nulle
        if (!isFollowing && !Mathf.Approximately(Time.time, rotationLerpStartTime) && Time.time <= rotationLerpStartTime + rotationLerpDuration)
        {
            float t = (Time.time - rotationLerpStartTime) / rotationLerpDuration;
            transform.localRotation = Quaternion.Slerp(initialRotation, Quaternion.identity, t);
        }
    }
}