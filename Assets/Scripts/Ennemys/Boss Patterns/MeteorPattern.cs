using UnityEngine;

public class MeteorPattern : MonoBehaviour
{
    public GameObject meteorPrefab; // Référence à l'objet que vous voulez instancier
    public float horizontalDistance = 20f; // Distance horizontale fixe entre le joueur et l'objet
    public float verticalDistance = 20f; // Distance verticale par rapport au sol
    public float meteorSpeed = 10f; // Vitesse du météore

    // Méthode pour obtenir la position du sol sous le joueur
    Vector3 GetGroundPosition(GameObject player)
    {
        RaycastHit hit;
        Vector3 groundPosition = player.transform.position;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            groundPosition = hit.point; // Position précise du sol sous le joueur
        }
        return groundPosition;
    }

    public void LaunchMeteorPattern()
    {
        // Trouver la position du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Trouver la direction dans laquelle le joueur est orienté
        Vector3 playerDirection = player.transform.forward;

        // Ajuster la position du joueur en fonction de la distance horizontale
        Vector3 playerPosition = player.transform.position;
        Vector3 spawnOffset = playerDirection * horizontalDistance;
        playerPosition += spawnOffset;

        // Trouver la position du sol sous le joueur ajusté
        RaycastHit hit;
        if (Physics.Raycast(playerPosition, Vector3.down, out hit))
        {
            // Vérifie si le tag du collider est "Sol". Assurez-vous que votre sol a le bon tag.
            if (hit.collider.CompareTag("Ground"))
            {
                // Calculer la position de l'objet à instancier
                Vector3 spawnPosition = hit.point + playerDirection * horizontalDistance + Vector3.up * verticalDistance;
                GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

                // Obtenir la position du sol sous le joueur
                Vector3 groundPosition = GetGroundPosition(player);

                // Calculer la direction vers le sol sous le joueur
                Vector3 moveDirection = (groundPosition - spawnPosition).normalized;

                // Donner une vitesse à l'objet en direction du sol
                Rigidbody rb = meteor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Appliquer la vitesse de déplacement
                    rb.velocity = moveDirection * meteorSpeed;

                    // Désactiver la gravité
                    rb.useGravity = false;
                }
                else
                {
                    Debug.LogError("Meteor prefab does not have a Rigidbody component!");
                }
            }
        }
    }
}