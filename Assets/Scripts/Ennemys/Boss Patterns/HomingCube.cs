// HomingCube.cs
using UnityEngine;

public class HomingCube : MonoBehaviour
{
    public float homingSpeed;
    private Transform target;
    private float destroyDelay;

    public void SetTarget(Vector3 playerPosition)
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetDestroyDelay(float delay)
    {
        destroyDelay = delay;
    }

    public void SetSpeed(float speed)
    {
        homingSpeed = speed;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Suivi du joueur
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * homingSpeed * Time.deltaTime);

        // Destruction après le délai spécifié
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si le cube entre en collision avec le joueur, le détruire
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
