using UnityEngine;

public class CrazyBlock : MonoBehaviour
{
    public GameObject explosion;
    public float minBounceForce = 10f;
    public float maxBounceForce = 20f;
     public float maxHorizontalSpeed = .1f;
    public float bounceForce;

    private bool isKinematicDisabled = false;

    private void Start()
    {
        StartCoroutine(CheckKinematic());
        bounceForce = Random.Range(minBounceForce, maxBounceForce);
    }

    private System.Collections.IEnumerator CheckKinematic()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            yield return new WaitUntil(() => !rb.isKinematic);

            isKinematicDisabled = true;
            float randomDelay = Random.Range(5f, 8f);
            yield return new WaitForSeconds(randomDelay);
            Explode();
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (isKinematicDisabled && collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    
                    rb.AddForce(direction * bounceForce, ForceMode.Impulse);
                }
            }
        }
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        if (isKinematicDisabled && collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate the direction towards the player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Patterns/Meteor_Bounce", GetComponent<Transform>().position);
                    Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

                    // Limit horizontal speed
                    Vector3 horizontalDirection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z).normalized;
                    Vector3 finalDirection = horizontalDirection * maxHorizontalSpeed + Vector3.up; // Adding Vector3.up to ensure the cube still moves upward

                    // Add force in the final direction
                    rb.AddForce(finalDirection * bounceForce, ForceMode.Impulse);
                }
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}