using UnityEngine;

public class ExplosiveBlock : MonoBehaviour
{
    private Vector3 lastPosition;

    public GameObject explosion;
    public float explosionTime = 1f;

    private void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(CheckMovement());
    }

    private System.Collections.IEnumerator CheckMovement()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return waitTime;
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
            }
            else
            {
                yield return new WaitForSeconds(explosionTime);
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
                yield break;
            }
        }
    }
}