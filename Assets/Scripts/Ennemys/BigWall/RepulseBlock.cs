using UnityEngine;

public class RepulseObject : MonoBehaviour
{
    // Forces de répulsion
    public GameObject child;
    public float oppositeForce = 2f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;
    public float repulsionForceHorizontal;

    // Fonction appelée lorsqu'une collision se produit
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 repulsionDirection = (collision.transform.position - transform.position).normalized;

        if (child != null)
        {
            Vector3 oppositeDirection = (child.transform.position - collision.transform.position).normalized;
            repulsionDirection += oppositeDirection * oppositeForce;
        }

        Vector3 horizontalDirection = Vector3.ProjectOnPlane(repulsionDirection, Vector3.up).normalized;
        Vector3 finalHorizontalDirection = horizontalDirection * repulsionForceHorizontal;
        Vector3 backwardDirection = -collision.transform.forward * backwardForce;
        Vector3 finalDirection = finalHorizontalDirection + Vector3.up * upwardForce + backwardDirection;

        Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(finalDirection, ForceMode.Impulse);
        }
    }
}