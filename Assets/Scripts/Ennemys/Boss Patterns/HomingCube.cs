using UnityEngine;

public class HomingCube : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 2f;
    public float rotationSpeed = 1f;
    public Transform target;
    private float destroyDelay;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Transform audioChild = transform.Find("Audio");
        if (audioChild != null)
        {
            audioChild.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 toTarget = target.position - rb.position;
            Vector3 direction = toTarget.normalized;
            Quaternion targetRotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            rb.MoveRotation(targetRotation);
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = transform.forward * speed;
        }
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0f)
        {
            Destroy(gameObject);
        }

        Debug.DrawRay(rb.position, rb.velocity, Color.green);
    }


    void OnDrawGizmos()
    {
        if (target == null) return;
        Gizmos.color = new Color(1f, 0.51f, 0.47f);
        Gizmos.DrawLine(transform.position, target.position);
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetDestroyDelay(float delay)
    {
        destroyDelay = delay;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player");
            Destroy(gameObject);
        }
    }
}
