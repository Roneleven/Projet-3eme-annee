using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float delayBeforeDisappear = 1f;
    public float explosionForce = 10f;
    public float explosionRadius = 5f;

    private void Start()
    {
        StartCoroutine(DisappearCoroutine());
    }

    private System.Collections.IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Calculate direction from the explosion to the player
                Vector3 explosionDirection = other.transform.position - transform.position;

                // Apply force to the player
                playerRigidbody.AddForce(explosionDirection.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }
}