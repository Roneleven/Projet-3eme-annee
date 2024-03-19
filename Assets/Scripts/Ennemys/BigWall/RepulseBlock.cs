using UnityEngine;

public class RepulseObject : MonoBehaviour
{
    // Force de répulsion
    public float repulsionForce = 10f;

    // Rayon de détection du joueur
    public float playerDetectionRadius = 1.5f;

    void Update()
    {
        // Vérifier si le joueur est en contact avec l'objet
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerDetectionRadius);
        foreach (Collider collider in colliders)
        {
            // Vérifier si le collider appartient au joueur
            if (collider.CompareTag("Player"))
            {
                // Calculer la direction du joueur par rapport à l'objet
                Vector3 repulsionDirection = (collider.transform.position - transform.position).normalized;

                // Appliquer la force de répulsion au joueur
                Rigidbody playerRigidbody = collider.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(repulsionDirection * repulsionForce, ForceMode.Impulse);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dessiner une sphère de détection pour visualiser la portée de l'objet
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}