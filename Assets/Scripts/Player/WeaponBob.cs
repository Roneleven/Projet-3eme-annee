using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float damping = 0.1f; // new damping factor for smoother transition

    private float timer = 0.0f;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
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
            timer = timer + bobbingSpeed * Time.deltaTime; // use Time.deltaTime for frame rate independence
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
            translateChange = totalAxes * translateChange * damping; // apply damping

            localPosition.y += translateChange;
        }
        else
        {
            localPosition.y = Mathf.Lerp(localPosition.y, originalLocalPosition.y, Time.deltaTime * damping); // smoothly return to the original position
        }

        transform.localPosition = localPosition;
    }
}
