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

        if (player == null)
        {
            Debug.LogWarning("Player not found. Make sure you have a GameObject tagged 'Player' in your scene.");
            return;
        }

        StartCoroutine(SpawnWallPattern());
    }

    private IEnumerator SpawnWallPattern()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(spawnInterval);

        while (true)
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 heartPosition = transform.position;
            
            Vector3 middlePoint = (playerPosition + heartPosition) / 2f;
            Vector3 spawnPosition = middlePoint + (heartPosition - middlePoint).normalized * 10f;
            spawnPosition.y = playerPosition.y;

            GameObject wallPatternInstance = Instantiate(wallPatternPrefab, spawnPosition, Quaternion.identity);

            Vector3 directionToPlayer = (playerPosition - spawnPosition).normalized;
            Vector3 xzOnlyDirectionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(xzOnlyDirectionToPlayer, Vector3.up);
            wallPatternInstance.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y + 180f, 0);

            yield return waitInterval;
        }
    }
}