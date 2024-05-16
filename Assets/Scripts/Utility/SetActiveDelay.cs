using UnityEngine;
using System.Collections;

public class SetActiveDelay : MonoBehaviour
{
    public float activationDelay;

    void Start()
    {
        gameObject.SetActive(false);
        StartCoroutine(ActivateAfterDelay());
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        gameObject.SetActive(true);
    }
}