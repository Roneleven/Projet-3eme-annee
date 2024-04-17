using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float delayBeforeDisappear = 5f;
    public float scaleFactor = 1f;
    
    public GameObject eye;
    public float oppositeForce = 2f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;
    public float repulsionForceHorizontal;

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

        if (eye != null)
        {
            Vector3 oppositeDirection = (eye.transform.position - collision.transform.position).normalized;
            repulsionDirection += oppositeDirection * oppositeForce;
        }

        Vector3 horizontalDirection = Vector3.ProjectOnPlane(repulsionDirection, Vector3.up).normalized;
        Vector3 finalHorizontalDirection = horizontalDirection * repulsionForceHorizontal;
        Vector3 backwardDirection = -collision.transform.forward * backwardForce;
        Vector3 finalDirection = finalHorizontalDirection + Vector3.up * upwardForce + backwardDirection;

        Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(finalDirection, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
}