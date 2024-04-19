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
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 toTarget = target.position - rb.position;

            // On normalise la direction pour obtenir une vitesse constante
            Vector3 direction = toTarget.normalized;

            // On tourne le Rigidbody dans la direction de la cible à la vitesse rotationSpeed
            Quaternion targetRotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            rb.MoveRotation(targetRotation);

            // On applique une force d'accélération dans la direction de la cible
            rb.velocity = direction * speed;
        }
        else
        {
            // Si le script n'a pas de cible, le cube va simplement se déplacer tout droit :
            rb.velocity = transform.forward * speed;
        }

        // Destruction après le délai spécifié
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

    // Nouvelle fonction pour définir la cible
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Nouvelle fonction pour définir la vitesse
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
            Destroy(gameObject);
        }
    }
}
