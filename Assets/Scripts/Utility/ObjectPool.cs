using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;

    public int poolSize = 1000;

    List<GameObject> availableObjects = new List<GameObject>();
    //faire fonction init pour reset le state d'un object avant de le remettre dans la pool

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            go.transform.SetParent(transform);
            availableObjects.Add(go);
        }
    }

    public GameObject GetObject()
    {
        if(availableObjects.Count > 0)
        {
            GameObject go = availableObjects[0];
            availableObjects.RemoveAt(0);
            go.SetActive(true);
            go.transform.SetParent(null);
            return go;
        }else
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            return go;
        }
    }

    public void PoolObject(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
        availableObjects.Add(go);
    }
}
