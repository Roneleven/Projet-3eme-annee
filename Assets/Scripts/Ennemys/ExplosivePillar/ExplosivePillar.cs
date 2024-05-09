using UnityEngine;

public class ExplosivePillar : MonoBehaviour
{
    public Transform playerTransform;
    public float interval = 0.1f;
    public bool spawnAllAtOnce = false;
    public float moveSpeed = 3f;
    public float rotationSpeed = 90f;

    private Transform[] positions;
    private int currentIndex = 0;
    private bool allCubesSpawned = false;
    private Quaternion targetRotation;
    private bool isKinematicDisabled = false;

    private FMOD.Studio.EventInstance pillar;

    private void Start()
    {

        pillar = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/Pillar_Start");
        pillar.setParameterByName("Pattern", 0.0F);
        pillar.start();

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

        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 45f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void Update()
    {

    pillar.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        if (allCubesSpawned)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
             pillar.setParameterByName("Pattern", 1.0F);
            Destroy(gameObject, 20f);
        }
    }

    

    private void SpawnCube()
    {
        if (currentIndex < positions.Length)
        {
            Transform cubeTransform = positions[currentIndex];
            cubeTransform.gameObject.SetActive(true);

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
        // Vérifier si le transform existe encore
        if (position != null)
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
}

    private void FixedUpdate()
{
    if (isKinematicDisabled)
    {
        foreach (Transform position in positions)
        {
            // Vérifier si le transform existe encore
            if (position != null)
            {
                Rigidbody rb = position.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    EjectCube(rb);
                }
            }
        }
        isKinematicDisabled = false;
    }
}

    private void EjectCube(Rigidbody rb)
    {
        rb.AddForce(Vector3.up * moveSpeed, ForceMode.Impulse);
    }
}