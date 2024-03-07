using System.Collections;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float delayBeforeDisappear = 1f;

    void Start()
    {
        StartCoroutine(DisappearCoroutine());
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);
        Destroy(gameObject);
    }
}
