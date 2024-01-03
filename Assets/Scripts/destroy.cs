using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy : MonoBehaviour
{
    public float timeBeforeDestroy;

    void Start()
    {
        Invoke("DestroyObject", timeBeforeDestroy);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
