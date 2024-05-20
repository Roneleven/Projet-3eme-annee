using UnityEngine;

public class GhostController : MonoBehaviour
{
    // Public GameObject à instancier
    public GameObject ghost;
    public Transform spawnPoint;

    private bool ghostSpawned = false; // Variable pour suivre l'état de création du fantôme

    private void Update()
    {
        HeartSpawner spawnSettings = GetComponent<HeartSpawner>();

        if (spawnSettings != null) // Vérifier si le composant HeartSpawner est attaché
        {
            float spawnRadius = spawnSettings.spawnRadius;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            bool playerInRange = false; // Variable pour vérifier si un joueur est dans la zone

            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < (spawnRadius + 2)) // >> spawnRadius + 2 = cocon <<
                {
                    playerInRange = true;
                    break; // Sortir de la boucle dès qu'un joueur est dans la zone
                }
            }

            if (playerInRange && !ghostSpawned) // Si un joueur est dans la zone et aucun fantôme n'a été créé
            {
                Instantiate(ghost, spawnPoint.position, Quaternion.identity);
                ghostSpawned = true; // Mettre à jour l'état de création du fantôme
            }
            else if (!playerInRange && ghostSpawned) // Si aucun joueur n'est dans la zone mais un fantôme a été créé
            {
                DestroyAllGhosts(); // Détruire tous les fantômes créés
                ghostSpawned = false; // Réinitialiser l'état de création du fantôme
            }
        }
    }

    private void DestroyAllGhosts()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghostObj in ghosts)
        {
            Destroy(ghostObj); // Détruire tous les objets avec le tag "Ghost"
        }
    }
}