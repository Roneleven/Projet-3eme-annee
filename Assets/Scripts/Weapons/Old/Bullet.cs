using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private int penetrationCount;
    private GameObject cubeDeathPrefab;


    public void InitializeBullet(float bulletSpeed, float lifeTime, int bulletDamage, int bulletPenetrationCount, GameObject cubeDeath)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        penetrationCount = bulletPenetrationCount;
        cubeDeathPrefab = cubeDeath;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        CubeHealth cubeHealth = collision.gameObject.GetComponent<CubeHealth>();

        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(damage);
            penetrationCount--;

            if (cubeHealth.health <= 0)
            {
                if (!cubeHealth.IsDead()) // Ajoutez cette vérification
                {
                    Instantiate(cubeDeathPrefab, collision.gameObject.transform.position, Quaternion.identity);
                    cubeHealth.SetDead(true); // Définissez le statut du cube comme mort
                }

                Destroy(collision.gameObject);
            }

            if (penetrationCount <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
