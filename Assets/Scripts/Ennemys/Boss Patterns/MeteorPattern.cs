using UnityEngine;
using System.Collections;

public class MeteorPattern : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float meteorSpeed = 50f;
    public float spawnRadius = 20f; // Rayon de spawn
    public float spawnHeight = 100f; // Hauteur de spawn
    public int numbersOfMeteors = 3;

    public void LaunchMeteorPattern()
    {
        StartCoroutine(SpawnMeteors());
    }

    IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < numbersOfMeteors; i++)
        {
            float randomDelay = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(randomDelay);

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Obtenir une position aléatoire dans un rayon autour du joueur
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;

                // Position de spawn aléatoire autour du joueur
                Vector3 spawnPosition = player.transform.position + randomOffset;

                // Cast vers le bas pour obtenir la position du sol
                RaycastHit hit;
                if (Physics.Raycast(spawnPosition, Vector3.down, out hit))
                {
                    // S'assurer que la position de spawn est au-dessus du sol
                    if (hit.collider.CompareTag("Ground"))
                    {
                        // Position de spawn avec une hauteur fixe au-dessus du sol
                        Vector3 meteorPosition = hit.point + Vector3.up * spawnHeight;

                        // Instantiation de la météorite
                        GameObject meteor = Instantiate(meteorPrefab, meteorPosition, Quaternion.identity);
                        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();

                        // Ajout de la force vers le bas à la météorite
                        if (meteorRb != null)
                        {
                            meteorRb.AddForce(Vector3.down * meteorSpeed, ForceMode.VelocityChange);
                        }
                    }
                }
            }
        }
    }
}