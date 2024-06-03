using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnTriggerDestroy : MonoBehaviour
{
    public GameObject targetToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetToActivate.gameObject.SetActive(true);
            Destroy(this);
        }
    }
}
