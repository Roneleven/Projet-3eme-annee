using UnityEngine;

public class HomingCube : MonoBehaviour
{
    public string targetTag = "Player"; // Tag de la cible
    public float speed = 5f; // Vitesse de déplacement
    public Player playerScript;

    private Transform target; // Référence à la cible

    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        // Trouver la cible par son tag
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogError("Aucune cible avec le tag '" + targetTag + "' trouvée.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Rotation vers la cible
            Vector3 direction = target.position - transform.position;
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationToTarget, Time.deltaTime * speed);

            // Déplacement vers la cible
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}