using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float delayBeforeDisappear = 1f;

    private void Start()
    {
        StartCoroutine(DisappearCoroutine());
    }

    private System.Collections.IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);
        Destroy(gameObject);
    }
}