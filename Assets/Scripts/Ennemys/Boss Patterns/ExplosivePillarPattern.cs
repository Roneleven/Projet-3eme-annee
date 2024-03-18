using UnityEngine;

public class ExplosivePillarPattern : MonoBehaviour
{
    public GameObject emptyPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 7f;

    private GameObject playerObject;
    private LayerMask groundLayerMask;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player not found with tag: Player");
            return;
        }

        groundLayerMask = LayerMask.GetMask("Ground");
        InvokeRepeating("SpawnEmptyObject", 0f, spawnInterval);
    }

    private void SpawnEmptyObject()
    {
        Vector2 randomPoint2D = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPoint = new Vector3(randomPoint2D.x, 0f, randomPoint2D.y) + playerObject.transform.position;
        randomPoint.z += Random.Range(-spawnRadius, spawnRadius);

        Collider[] colliders = Physics.OverlapSphere(randomPoint, 20f, groundLayerMask);
        if (colliders.Length == 0)
        {
            Debug.Log("No ground found near player.");
            return;
        }

        if (!Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            Debug.LogError("Failed to find ground surface.");
            return;
        }

        GameObject emptyObject = Instantiate(emptyPrefab, hit.point, Quaternion.identity);
        Vector3 direction = playerObject.transform.position - hit.point;
        direction.y = 0f;
        emptyObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
