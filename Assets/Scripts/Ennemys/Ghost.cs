using UnityEngine;
using System.Collections;

public class SuivreJoueur : MonoBehaviour
{
    public string tagDuJoueur = "Player";
    public float vitesseDeDeplacement = 20f;
    public float scaleSpeed = 1f;

    public float oppositeForce = 2f;
    public float backwardForce = 10f;
    public float upwardForce = 1f;
    public float repulsionForceHorizontal;

    private Transform joueur;
    private bool estEnMouvement = true;

    private void Start()
    {
        joueur = GameObject.FindGameObjectWithTag(tagDuJoueur).transform;
    }

    private void Update()
    {
        if (estEnMouvement)
        {
            Vector3 direction = (joueur.position - transform.position).normalized;
            Vector3 deplacement = direction * vitesseDeDeplacement * Time.deltaTime;
            transform.Translate(deplacement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDuJoueur))
        {
            estEnMouvement = false;
            StartCoroutine(ExpandCoroutine());
            GetComponent<Collider>().isTrigger = false;
            
        }
    }

    private IEnumerator ExpandCoroutine()
    {
        transform.localScale = Vector3.zero;

        while (true)
        {
            Vector3 scaleChange = Vector3.one * scaleSpeed * Time.deltaTime;
            transform.localScale += scaleChange;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Vector3 repulsionDirection = (collision.transform.position - transform.position).normalized;
        GameObject heartObject = GameObject.FindGameObjectWithTag("Heart");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Patterns/Ghost");

        if (heartObject != null)
        {
            Vector3 oppositeDirection = (heartObject.transform.position - collision.transform.position).normalized;
            repulsionDirection += oppositeDirection * oppositeForce;
        }

        Vector3 horizontalDirection = Vector3.ProjectOnPlane(repulsionDirection, Vector3.up).normalized;
        Vector3 finalHorizontalDirection = horizontalDirection * repulsionForceHorizontal;
        Vector3 backwardDirection = -collision.transform.forward * backwardForce;
        Vector3 finalDirection = finalHorizontalDirection + Vector3.up * upwardForce + backwardDirection;

        Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(finalDirection, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
}