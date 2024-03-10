using UnityEngine;

public class CrazyBlock : MonoBehaviour
{
    public GameObject explosion;
    public float bounceForce = 10f;

    private bool isKinematicDisabled = false;

    private void Start()
    {
        StartCoroutine(CheckKinematic());
    }

    private System.Collections.IEnumerator CheckKinematic()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            yield return new WaitUntil(() => !rb.isKinematic);

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