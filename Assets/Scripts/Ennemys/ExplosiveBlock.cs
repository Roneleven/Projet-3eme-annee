using System.Collections;
using UnityEngine;

public class CubeBehavior : MonoBehaviour
{
    private Vector3 lastPosition;
    private bool isMoving;

    public GameObject explosion;

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(CheckMovement());
    }


    IEnumerator CheckMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
                isMoving = true;
            }
            else
            {
                isMoving = false;
                StartCoroutine(SpawnNewObjectAndDestroy());
                break;
            }
        }
    }

    IEnumerator SpawnNewObjectAndDestroy()
    {
        yield return new WaitForSeconds(2f);

        GameObject newObject = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}