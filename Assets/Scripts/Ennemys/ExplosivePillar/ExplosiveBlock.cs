using UnityEngine;

public class ExplosiveBlock : MonoBehaviour
{
    private Vector3 lastPosition;

    public GameObject explosion;

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
                yield return new WaitForSeconds(2f);
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
                yield break;
            }
        }
    }
}