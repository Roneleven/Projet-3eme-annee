using UnityEngine;
using System.Collections;

public class MeteorPattern : MonoBehaviour
{
    public GameObject objectToSpawn; // Le Game object à instancier
    public float spawnDistance = 20f; // Distance entre le joueur et le point de spawn
    public float spawnHeight = 20f; // La hauteur à laquelle instancier l'objet
    public float moveSpeed = 10f; // Vitesse de déplacement de l'objet
    private float spawnAngleRange = 30f;
    private LayerMask groundLayerMask;


public void LaunchMeteorPattern()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    groundLayerMask = LayerMask.GetMask("Ground");

        // Obtenir la position du sol sous le joueur
        RaycastHit hit;
        Vector3 groundPosition = player.transform.position;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit) && hit.collider.CompareTag("Ground"))
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