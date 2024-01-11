using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float damping = 0.1f;

    public Transform groundCheck; // Transform for where the ground check should start
    public float groundDistance = 0.1f; // Distance of the ground check raycast
    public LayerMask groundMask; // Layer mask to identify the ground

    private float timer = 0.0f;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Check if the player is grounded
        bool isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        if (!isGrounded) return; // Skip bobbing if not grounded

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
