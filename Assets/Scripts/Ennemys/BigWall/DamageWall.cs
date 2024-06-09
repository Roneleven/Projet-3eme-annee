using UnityEngine;

public class DamageWall : MonoBehaviour
{
    public float repulsionForce = 1f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;

    public Player playerScript;

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

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("HeartBlock")|| collision.gameObject.CompareTag("DestroyableBlock"))
        {
            Destroy(gameObject);
        }
    }
}