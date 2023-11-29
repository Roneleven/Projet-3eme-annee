using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private int penetrationCount;

    public void Initialize(float bulletSpeed, float lifeTime, int bulletDamage, int bulletPenetrationCount)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        penetrationCount = bulletPenetrationCount;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name + "; Penetration count before hit: " + penetrationCount);

        CubeHealth cubeHealth = collision.gameObject.GetComponent<CubeHealth>();
        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(damage);
            penetrationCount--;

            Debug.Log("Penetration count after hit: " + penetrationCount);

            if (penetrationCount <= 0)
            {
                Debug.Log("Bullet destroyed after hitting: " + collision.gameObject.name);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Bullet hit a non-penetrable object: " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }

}
