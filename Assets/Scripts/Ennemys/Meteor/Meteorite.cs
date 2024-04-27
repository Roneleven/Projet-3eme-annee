using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public Player playerScript;

    public GameObject objectToSpawnOnImpact;
    public GameObject feedbackObject;
    public float explosionRadius = 1.5f;
    private FMOD.Studio.EventInstance meteor;
    private GameObject feedbackInstance;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();

        Vector3 spawnPosition = transform.position;
        MeteorPattern meteorPattern = FindObjectOfType<MeteorPattern>();
        meteor = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/Meteor_Start");
        meteor.setParameterByName("Pattern", 0.0F);
        meteor.start();

        if (meteorPattern != null)
        {
            spawnPosition.y -= meteorPattern.spawnHeight;
        }

        feedbackInstance = Instantiate(feedbackObject, spawnPosition, Quaternion.identity);
    }

    void Update()
    {
        meteor.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Instantiate(objectToSpawnOnImpact, transform.position, Quaternion.identity);
            Destroy(gameObject);
            if (feedbackInstance != null)
            {
                meteor.setParameterByName("Pattern", 1.0F);
                Destroy(feedbackInstance);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hitCollider in colliders)
            {
                if (hitCollider.CompareTag("Block") || hitCollider.CompareTag("DestroyableBlock"))
                {
                    Destroy(hitCollider.gameObject);
                }
            }


        }
        
        if (collision.collider.CompareTag("Block") || collision.collider.CompareTag("DestroyableBlock"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                meteor.setParameterByName("Pattern", 2.0F);
                meteor.setParameterByName("Pattern", 0.0F);
                rb.useGravity = true;
                rb.velocity = new Vector3(rb.velocity.x, 20f, rb.velocity.z);
            }
        }

        if (collision.collider.CompareTag("HeartBlock"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                meteor.setParameterByName("Pattern", 2.0F);
                meteor.setParameterByName("Pattern", 0.0F);
                rb.useGravity = true;
                rb.velocity = new Vector3(rb.velocity.x, 2f, rb.velocity.z);
            }

            Destroy(collision.collider.gameObject);
        }

        if (collision.collider.CompareTag("Player"))
        {
                playerScript.TakeDamage(70);
                Destroy(gameObject);
                Destroy(feedbackInstance);
        }
    }
}