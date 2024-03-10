using System.Collections;
using UnityEngine;

public class CrazyBlock : MonoBehaviour
{
    public GameObject explosion;
    public float bounceForce = 10f; // Force de rebondissement

    private bool isKinematicDisabled = false;

    void Start()
    {
        StartCoroutine(CheckKinematic());
    }

    IEnumerator CheckKinematic()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            while (rb.isKinematic)
            {
                yield return null;
            }

            isKinematicDisabled = true;
            float randomDelay = Random.Range(5f, 8f);
            yield return new WaitForSeconds(randomDelay);
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isKinematicDisabled && collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Ajouter une force vers le haut pour simuler le rebondissement
                rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }
        }
    }

    private void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}