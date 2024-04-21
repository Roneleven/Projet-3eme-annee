using UnityEngine;

public class DamageBlock : MonoBehaviour
{
    public float repulsionForce = 1f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;

    public Player playerScript;

    private void Start()
    {
        Destroy(gameObject, .5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 repulsionDirection = (collision.transform.position - transform.position).normalized;

            repulsionDirection -= transform.forward * backwardForce;
            repulsionDirection += transform.up * upwardForce;

            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(repulsionDirection * repulsionForce, ForceMode.Impulse);
            }

            playerScript.TakeDamage(10);

        Destroy(gameObject);
        }
    }
}