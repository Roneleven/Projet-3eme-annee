using UnityEngine;
using System.Collections;

public class BigWallPattern : MonoBehaviour
{
    public GameObject wallPatternPrefab;


    public void LaunchWallPattern()
    {

            GameObject player = GameObject.FindGameObjectWithTag("Player");

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
    }
}