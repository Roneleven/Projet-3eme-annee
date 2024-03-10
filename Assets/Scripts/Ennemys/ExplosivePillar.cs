using UnityEngine;

public class ExplosivePillar : MonoBehaviour
{
    public Transform playerTransform; 
    public float interval = 0.1f;
    public bool spawnAllAtOnce = false; 
    public float moveSpeed = 3f;
    public float rotationSpeed = 90f; 

    private int currentIndex = 0;
    private Transform[] positions;
    private bool allCubesSpawned = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isKinematicDisabled = false;

    private void Start()
    {
        positions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i);
            positions[i].gameObject.SetActive(false);
        }

        if (spawnAllAtOnce)
        {
            SpawnAllCubes();
            ActivateExplosiveBlocksAndRigidbodies();
        }
        else
        {
            InvokeRepeating("SpawnCube", interval, interval);
        }

        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x + 45f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
    }

    private void Update()
    {
        if (allCubesSpawned)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Destroy(gameObject, 20f); // Destruction après 20 secondes
        }
    }

    private void SpawnCube()
    {
        if (currentIndex < positions.Length)
        {
            Transform cubeTransform = positions[currentIndex];
            cubeTransform.gameObject.SetActive(true);
            
            // Orienter le cube vers le joueur sur l'axe Y
            if (playerTransform != null)
            {
                Vector3 direction = playerTransform.position - cubeTransform.position;
                direction.y = 0f;
                cubeTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            currentIndex++;
        }
        else
        {
            CancelInvoke("SpawnCube");
            allCubesSpawned = true;
            ActivateExplosiveBlocksAndRigidbodies();
        }
    }

    private void SpawnAllCubes()
    {
        foreach (Transform position in positions)
        {
            position.gameObject.SetActive(true);
        }
        allCubesSpawned = true;
    }

    private void ActivateExplosiveBlocksAndRigidbodies()
    {
        foreach (Transform position in positions)
        {
            CrazyBlock explosiveBlock = position.GetComponent<CrazyBlock>();
            if (explosiveBlock != null)
            {
                explosiveBlock.enabled = true;
            }

            Rigidbody rb = position.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                isKinematicDisabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isKinematicDisabled)
        {
            foreach (Transform position in positions)
            {
                Rigidbody rb = position.GetComponent<Rigidbody>();
                if (rb != null && rb.isKinematic == false)
                {
                    EjectCube(position);
                }
            }
            isKinematicDisabled = false;
        }
    }

    private void EjectCube(Transform cubeTransform)
    {
        Rigidbody rb = cubeTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * moveSpeed, ForceMode.Impulse);
        }
    }
}