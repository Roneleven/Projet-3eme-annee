using UnityEngine;

public class MeteorPattern : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float horizontalDistance = 20f; 
    public float verticalDistance = 20f;
    public float meteorSpeed = 10f; 

    Vector3 GetGroundPosition(GameObject player)
    {
        RaycastHit hit;
        Vector3 groundPosition = player.transform.position;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            groundPosition = hit.point;
        }
        return groundPosition;
    }

    public void LaunchMeteorPattern()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        Vector3 playerDirection = player.transform.forward;

        Vector3 playerPosition = player.transform.position;
        Vector3 spawnOffset = playerDirection * horizontalDistance;
        playerPosition += spawnOffset;

        RaycastHit hit;
        if (Physics.Raycast(playerPosition, Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 spawnPosition = hit.point + playerDirection * horizontalDistance + Vector3.up * verticalDistance;
                GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

                Vector3 groundPosition = GetGroundPosition(player);

                Vector3 moveDirection = (groundPosition - spawnPosition).normalized;

                Rigidbody rb = meteor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = moveDirection * meteorSpeed;

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