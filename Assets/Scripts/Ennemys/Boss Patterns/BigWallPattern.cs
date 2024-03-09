using UnityEngine;
using System.Collections;

public class BigWallPattern : MonoBehaviour
{
    public GameObject wallPatternPrefab;
    public float spawnInterval = 6f;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnWallPattern());
    }

    private IEnumerator SpawnWallPattern()
    {
        while (true)
        {
            // Récupérer les positions du joueur et du coeur (c'est-à-dire de cet objet)
            Vector3 playerPosition = player.transform.position;
            Vector3 heartPosition = transform.position;

            // Calculer le milieu entre le joueur et le coeur
            Vector3 middlePoint = (playerPosition + heartPosition) / 2f;

            // Ajouter une distance de 10 unités dans la direction du coeur
            Vector3 spawnPosition = middlePoint + (heartPosition - middlePoint).normalized * 10f;

            // Utiliser la position calculée pour instancier l'objet, en gardant la hauteur Y du joueur
            spawnPosition.y = playerPosition.y;

            GameObject wallPatternInstance = Instantiate(wallPatternPrefab, spawnPosition, Quaternion.identity);

            Vector3 directionToPlayer = (playerPosition - spawnPosition).normalized;

            // Créer un vecteur qui contient seulement les composantes X et Z de la direction vers le joueur
            Vector3 xzOnlyDirectionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;

            // Utiliser LookRotation avec le vecteur xzOnlyDirectionToPlayer pour obtenir une rotation sur les axes X et Z,
            // puis ajouter 180 degrés pour inverser la direction
            Quaternion targetRotation = Quaternion.LookRotation(xzOnlyDirectionToPlayer, Vector3.up);
            wallPatternInstance.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y + 180f, 0);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}