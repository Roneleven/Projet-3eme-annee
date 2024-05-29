using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    private float explosionRadius;
    private GameObject explosionPrefab;
    private int bulletDamage;

    public void InitializeExplosive(float radius, GameObject prefab, int damage)
    {
        explosionRadius = radius;
        explosionPrefab = prefab;
        bulletDamage = damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        CubeHealth cubeHealth = collision.gameObject.GetComponent<CubeHealth>();

        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(bulletDamage, true);
        }

        Explode();
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            CubeHealth cubeHealth = collider.GetComponent<CubeHealth>();

            if (cubeHealth != null)
            {
                cubeHealth.TakeDamage(bulletDamage, true);
            }
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
