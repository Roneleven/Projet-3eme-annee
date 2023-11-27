using UnityEngine;

public class BulletBaseBehavior : MonoBehaviour
{
    public float speed = 100f;
    public Rigidbody rb;

    void Start()
    {
        // Utilisez la direction de la caméra pour déterminer la direction de la balle
        Vector3 direction = Camera.main.transform.forward;
        rb.velocity = direction * speed;

        // Détruit la balle après 2 secondes
        Invoke("DestroyBullet", 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        CubeHealth cubeHealth = collision.gameObject.GetComponent<CubeHealth>();
        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(1);
            // Annule l'invocation si la balle est détruite lors d'une collision
            CancelInvoke("DestroyBullet");
            Destroy(gameObject);
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}