using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeImpulsePattern : MonoBehaviour
{
    private HeartSpawner heartSpawner;
    public float launchForce;
    public float explosionRadius;

    private void Start()
    {
        heartSpawner = GetComponent<HeartSpawner>();
        StartCoroutine(LaunchCubePeriodically());
    }

    private IEnumerator LaunchCubePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            Vector3 playerPosition = heartSpawner.playerPosition;

            Vector3 launchDirection = (playerPosition - transform.position).normalized;

            GameObject cubeToLaunch = Instantiate(heartSpawner.cubePrefab, heartSpawner.transform.position, Quaternion.identity, heartSpawner.spawnContainer.transform);

            Rigidbody cubeRigidbody = cubeToLaunch.AddComponent<Rigidbody>();
            cubeRigidbody.useGravity = true;

            cubeRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Destroy(cubeToLaunch, heartSpawner.cubeDestroyDelay);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
        Debug.Log("Object Tag: " + collision.gameObject.tag);
        Debug.Log("Object Layer: " + collision.gameObject.layer);

        // Check if the collision is with the player or the ground
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Player or Ground collision detected");

            // Create an explosion at the collision point
            CreateExplosion(collision.contacts[0].point);

            // Apply impulse to the player if it's a player collision
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Applying impulse to player");
                ApplyImpulseToPlayer(collision.transform);
            }
        }
    }



    private void CreateExplosion(Vector3 explosionPoint)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                ApplyImpulseToPlayer(collider.transform);
            }
        }
    }

    private void ApplyImpulseToPlayer(Transform playerTransform)
    {
        Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, explosionRadius);
    }
}
