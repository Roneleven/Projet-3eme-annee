using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private Animator Door = null;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isClosed = false;
    public GameObject unlinkedBoxSpawnersToRemove;

    private void OnTriggerEnter(Collider other)
    {
        if (isClosed)
            {
                Door.Play("DoorClose");
                gameObject.SetActive(false);
            }
            Destroy(unlinkedBoxSpawnersToRemove);
    }
}

