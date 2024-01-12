using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float damping = 0.1f;

    public bool enableGroundCheck = true; // Toggle this in the editor to enable/disable ground check
    public Transform groundCheck; 
    public float groundDistance = 0.1f; 
    public LayerMask groundMask; 

    private float timer = 0.0f;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        bool isGrounded = true; // Assume grounded by default

        // Perform ground check if enabled
        if (enableGroundCheck)
        {
            isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);
        }

        // Skip bobbing if not grounded and ground check is enabled
        if (enableGroundCheck && !isGrounded) return;

        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }

        Vector3 localPosition = originalLocalPosition;

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange * damping;

            localPosition.y += translateChange;
        }
        else
        {
            localPosition.y = Mathf.Lerp(localPosition.y, originalLocalPosition.y, Time.deltaTime * damping);
        }

        transform.localPosition = localPosition;
    }
}
