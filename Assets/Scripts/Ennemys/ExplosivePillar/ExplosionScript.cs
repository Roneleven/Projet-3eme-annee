using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float delayBeforeDisappear = 5f;
    public float scaleFactor = 1f;
    public float repulsionForce = 2f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;

    private void Start()
    {
        StartCoroutine(ExpandCoroutine());
        StartCoroutine(DisappearCoroutine());
    }

    private System.Collections.IEnumerator ExpandCoroutine()
    {
        // Initialiser l'échelle à 0
        transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < delayBeforeDisappear)
        {
            // Calculer le facteur d'échelle en fonction du temps écoulé
            float scale = Mathf.Lerp(0f, scaleFactor, timer / delayBeforeDisappear);
            // Appliquer l'échelle
            transform.localScale = new Vector3(scale, scale, scale);

            // Incrémenter le timer
            timer += Time.deltaTime;

            yield return null;
        }

        // Assurer que l'échelle finale est bien appliquée
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    private System.Collections.IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 repulsionDirection = (collision.transform.position - transform.position).normalized;

            repulsionDirection -= transform.forward * backwardForce;
            repulsionDirection += transform.up * upwardForce;

            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(repulsionDirection * repulsionForce, ForceMode.Impulse);
            }

        Destroy(gameObject);
        }
    }
}