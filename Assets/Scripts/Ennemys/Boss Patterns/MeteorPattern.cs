using UnityEngine;
using System.Collections;

public class MeteorPattern : MonoBehaviour
{
    public GameObject objectToSpawn; // Le Game object à instancier
    private string groundTag = "Ground"; // Le tag à rechercher pour le sol
    private string playerTag = "Player"; // Le tag à rechercher pour le joueur
    public float spawnDistance = 20f; // Distance entre le joueur et le point de spawn
    public float spawnHeight = 20f; // La hauteur à laquelle instancier l'objet
    public float launchInterval = 11f; // L'intervalle entre chaque lancement
    public float moveSpeed = 10f; // Vitesse de déplacement de l'objet
    private float spawnAngleRange = 45f;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(launchInterval);

            // Vérifier si le joueur est au-dessus du sol
            if (IsPlayerAboveGround())
            {
                // Créer un nouvel objet
                SpawnObject();
            }
        }
    }

    bool IsPlayerAboveGround()
    {
        // Trouver l'objet joueur dans la scène
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            // Vérifier s'il y a un objet avec le tag "Ground" sous le joueur
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit) && hit.collider.CompareTag(groundTag))
            {
                return true; // Le joueur est au-dessus du sol
            }
        }
        return false; // Le joueur n'est pas au-dessus du sol ou n'a pas été trouvé
    }

void SpawnObject()
{
    // Trouver l'objet joueur dans la scène
    GameObject player = GameObject.FindWithTag(playerTag);
    if (player != null)
    {
        // Obtenir la position du sol sous le joueur
        RaycastHit hit;
        Vector3 groundPosition = player.transform.position;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit) && hit.collider.CompareTag(groundTag))
        {
            groundPosition = hit.point; // Position précise du sol sous le joueur
        }

        // Obtenir la direction dans laquelle le joueur regarde
        Vector3 spawnDirection = player.transform.forward;

        // Calculer un angle aléatoire dans une fourchette donnée
        float randomAngle = Random.Range(-spawnAngleRange, spawnAngleRange);

        // Appliquer cet angle à la direction de spawn
        Quaternion spawnRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        spawnDirection = spawnRotation * spawnDirection;

        // Calculer la position de spawn devant le joueur
        Vector3 spawnPosition = player.transform.position + spawnDirection * spawnDistance + Vector3.up * spawnHeight;

        // Instancier l'objet à la nouvelle position
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

        // Calculer la direction vers le sol sous le joueur
        Vector3 moveDirection = (groundPosition - spawnPosition).normalized;

        // Donner une vitesse à l'objet en direction du sol
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Appliquer la vitesse de déplacement
            rb.velocity = moveDirection * moveSpeed;

            // Désactiver la gravité
            rb.useGravity = false;
        }
    }
}
}