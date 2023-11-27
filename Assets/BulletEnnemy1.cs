using UnityEngine;

public class BulletEnnemy1 : MonoBehaviour
{
    public float speed = 20f;
    private Vector3 direction;

    public void Initialize(Vector3 direction)
    {
        this.direction = direction;
        // Détruit la balle après 5 secondes
        Invoke("DestroyBullet", 5f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Vérifie si la balle est entrée en collision avec le joueur
        if (collision.gameObject.tag == "Player")
        {
            // Annule l'invocation si la balle est détruite lors d'une collision
            CancelInvoke("DestroyBullet");
            Destroy(gameObject);
        }
    }

    void DestroyBullet()
    {
        // Vérifie si l'objet existe toujours avant de le détruire
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}