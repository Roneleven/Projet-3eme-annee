using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWall : MonoBehaviour
{
    public Player playerScript;
    public float speed = 10f; // Vitesse de déplacement
    public Material newMaterial;
    public GameObject childObject;

    private bool hasBeenPropelled = false; // Variable pour suivre l'état de propulsion
    private Transform playerTransform;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        if (playerScript != null)
        {
            playerTransform = playerScript.transform;
        }
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
            ChangeMaterial();
            Invoke("StartPropulsion", 3f);
        }
    }

    void StartPropulsion()
    {
        hasBeenPropelled = true;
        Invoke("DestroyBlock", 3f); // Détruire l'objet après 3 secondes
    }

    void Update()
    {
        if (hasBeenPropelled && playerTransform != null)
        {
            // Calculer la direction vers le joueur
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Déplacer l'objet vers le joueur
            transform.position += direction * speed * Time.deltaTime;

            // Orienter l'objet vers le joueur
            transform.LookAt(playerTransform);
        }
    }

    void ChangeMaterial()
    {
        if (childObject != null)
        {
            Renderer renderer = childObject.GetComponent<Renderer>();
            if (renderer != null && newMaterial != null)
            {
                renderer.material = newMaterial;
            }
        }
        else
        {
            Debug.LogWarning("Child object not assigned.");
        }
    }

    void DestroyBlock()
    {
        Destroy(gameObject);
    }
}