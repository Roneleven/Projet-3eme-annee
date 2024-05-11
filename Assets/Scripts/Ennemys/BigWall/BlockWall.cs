using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWall : MonoBehaviour
{
    public Player playerScript;
    public GameObject losangePrefab;
    public float forceMagnitude;

    private bool hasBeenPropelled = false; // Variable pour suivre l'état de propulsion

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasBeenPropelled && collision.gameObject.CompareTag("Player"))
        {
            playerScript.TakeDamage(10);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else
        {
            transform.SetParent(null);
            Invoke("SpawnLosange", 3f);
        }
    }

    void SpawnLosange()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.isKinematic = false;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 direction = player.transform.position - transform.position;
                float distance = direction.magnitude;
                float calculatedForce = forceMagnitude * distance;
                rb.AddForce(direction.normalized * calculatedForce, ForceMode.Impulse);
            }
        }

        Invoke("DestroyBlock", 3f);
        hasBeenPropelled = true; // Marque l'objet comme ayant été propulsé
    }

    void DestroyBlock()
    {
        Destroy(gameObject);
    }
}