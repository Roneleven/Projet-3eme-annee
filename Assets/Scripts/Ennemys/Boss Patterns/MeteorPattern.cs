using UnityEngine;
using System.Collections;

public class MeteorPattern : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float meteorSpeed = 50f;
    public float spawnRadius = 20f; // Rayon de spawn
    public float spawnHeight = 100f; // Hauteur de spawn

    public void LaunchMeteorPattern()
    {
        StartCoroutine(SpawnMeteors());
    }

    IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < 10; i++)
        {
            float randomDelay = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(randomDelay);

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                Vector3 spawnPosition = player.transform.position + randomOffset;
                RaycastHit hit;
                if (Physics.Raycast(spawnPosition, Vector3.down, out hit))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        Vector3 meteorPosition = hit.point + Vector3.up * spawnHeight;
                        GameObject meteor = Instantiate(meteorPrefab, meteorPosition, Quaternion.identity);
                        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();
                        
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