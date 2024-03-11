using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public GameObject objectToSpawnOnImpact; // Le Game object à instancier au moment de l'impact avec le sol

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifier si l'objet a collisionné avec un objet portant le tag "Ground"
        if (collision.collider.CompareTag("Ground"))
        {
            // Créer un nouvel objet à la même position que l'objet qui tombe
            Instantiate(objectToSpawnOnImpact, transform.position, Quaternion.identity);

            // Détruire cet objet qui tombe
            Destroy(gameObject);
        }
    }
}