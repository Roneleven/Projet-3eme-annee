using UnityEngine;

public class RepulseObject : MonoBehaviour
{
    // Forces de répulsion
    public float repulsionForce = 1f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;

    // Fonction appelée lorsqu'une collision se produit
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Calculer la direction de la répulsion
            Vector3 repulsionDirection = (collision.transform.position - transform.position).normalized;

            // Ajouter les forces de répulsion
            repulsionDirection -= transform.forward * backwardForce;
            repulsionDirection += transform.up * upwardForce;

            // Appliquer la force de répulsion au joueur
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(repulsionDirection * repulsionForce, ForceMode.Impulse);
            }
        }
    }
}