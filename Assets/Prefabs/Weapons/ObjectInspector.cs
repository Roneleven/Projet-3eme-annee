using UnityEngine;

public class ObjectInspector : MonoBehaviour
{
    public Transform target; // The object to inspect
    public float rotationSpeed = 100f;
    public float zoomSpeed = 10f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private float distance;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target set for ObjectInspector.");
            enabled = false;
            return;
        }

        distance = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {
        RotateObject();
        ZoomObject();
    }

    void RotateObject()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            target.Rotate(Vector3.up, horizontal, Space.World);
            target.Rotate(Vector3.right, vertical, Space.World);
        }
    }

    void ZoomObject()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance - scroll, minZoom, maxZoom);

        Vector3 direction = (transform.position - target.position).normalized;
        transform.position = target.position + direction * distance;
    }
}
