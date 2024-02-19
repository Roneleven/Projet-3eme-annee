using UnityEngine;

public class BulletBaseBehavior : MonoBehaviour
{
    public float speed = 100f;
    public Rigidbody rb;

    void Start()
    {
        // Détruit la balle après 2 secondes
        Invoke("DestroyBullet", 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Recherche le composant HeartHealth sur l'objet touché
        HeartHealth heartHealth = collision.gameObject.GetComponent<HeartHealth>();

        // Si le composant HeartHealth est trouvé, inflige des dégâts au coeur
        if (heartHealth != null)
        {
            heartHealth.TakeDamage(1);
        }

        // Recherche le composant CubeHealth sur l'objet touché
        CubeHealth cubeHealth = collision.gameObject.GetComponent<CubeHealth>();

        // Si le composant CubeHealth est trouvé, inflige des dégâts au cube
        if (cubeHealth != null)
        {
            cubeHealth.TakeDamage(1);
        }

        // Annule l'invocation si la balle est détruite lors d'une collision
        CancelInvoke("DestroyBullet");
        Destroy(gameObject);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
