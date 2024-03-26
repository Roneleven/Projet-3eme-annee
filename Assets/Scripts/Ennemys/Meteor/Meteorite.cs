using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public GameObject objectToSpawnOnImpact; 
    public float destructionRadius = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Instantiate(objectToSpawnOnImpact, transform.position, Quaternion.identity);

            Destroy(gameObject);

            Collider[] colliders = Physics.OverlapSphere(transform.position, destructionRadius);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Block") || col.CompareTag("DestroyableBlock") || col.CompareTag("HeartBlock"))
                {
                    // Détruire l'objet
                    Destroy(col.gameObject);
                }
            }
        }
    }
}