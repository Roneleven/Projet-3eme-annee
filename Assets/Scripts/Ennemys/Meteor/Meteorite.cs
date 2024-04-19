using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public GameObject objectToSpawnOnImpact; // Le Game object à instancier au moment de l'impact avec le sol
    public GameObject feedbackObject; // Le Game object "Feedback" à instancier avec la météorite
    public float explosionRadius = 1.5f; // Rayon de l'explosion

    private GameObject feedbackInstance; // Référence à l'instance du feedback

    private void Start()
    {
        // Récupérer la position de l'objet qui possède le script MeteorPattern
        Vector3 spawnPosition = transform.position;
        MeteorPattern meteorPattern = FindObjectOfType<MeteorPattern>();
        if (meteorPattern != null)
        {
            // Ajuster la hauteur de spawn
            spawnPosition.y -= meteorPattern.spawnHeight;
        }

        // Instancier l'objet "Feedback" à la position calculée et stocker la référence à son instance
        feedbackInstance = Instantiate(feedbackObject, spawnPosition, Quaternion.identity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifier si l'objet a collisionné avec un objet portant le tag "Ground"
        if (collision.collider.CompareTag("Ground"))
        {
            // Créer un nouvel objet à la même position que l'objet qui tombe
            Instantiate(objectToSpawnOnImpact, transform.position, Quaternion.identity);

            // Détruire cet objet qui tombe
            Destroy(gameObject);
            
            // Détruire également le feedback associé
            if (feedbackInstance != null)
            {
                Destroy(feedbackInstance);
            }

            // Effectuer une détection de collision dans le rayon d'explosion
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hitCollider in colliders)
            {
                // Vérifier si l'objet dans le rayon possède les tags "Block" ou "DestroyableBlock"
                if (hitCollider.CompareTag("Block") || hitCollider.CompareTag("DestroyableBlock"))
                {
                    // Détruire l'objet
                    Destroy(hitCollider.gameObject);
                }
            }
        }
        
        // Vérifier si l'objet a collisionné avec un objet portant le tag "Block" ou "DestroyableBlock" (sans utiliser le rayon d'explosion)
        if (collision.collider.CompareTag("Block") || collision.collider.CompareTag("DestroyableBlock"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true; // Activer la gravité sur cet objet
                rb.velocity = new Vector3(rb.velocity.x, 20f, rb.velocity.z); // Ajouter une impulsion vers le haut
            }
        }
    }
}