using UnityEngine;

public class BigWall : MonoBehaviour
{
    public GameObject cubePrefab;
    public float interval = 0.1f;
    public float moveSpeed = 1f; 

    private int currentIndex = 0;
    private Transform[] positions;
    private bool allCubesSpawned = false;
    private Transform wallPatternTransform;

    private void Start()
    {
        positions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i);
        }

        foreach (Transform position in positions)
        {
            position.gameObject.SetActive(false);
        }

        wallPatternTransform = GetComponent<Transform>();

        InvokeRepeating("SpawnCube", interval, interval);
    }

    private void Update()
    {
        if (allCubesSpawned && !wallPatternTransform.GetComponent<WallPatternMovement>())
        {
            wallPatternTransform.gameObject.AddComponent<WallPatternMovement>().Initialize(moveSpeed);
            // Détruire l'objet et ses enfants après 20 secondes de déplacement
            Invoke("DestroyWallPattern", 10f);
        }
    }

    private void DestroyWallPattern()
    {
        // Détruire l'objet WallPattern2 et ses enfants (les cubes)
        Destroy(gameObject);
    }

    private void SpawnCube()
    {
        if (currentIndex < positions.Length)
        {
            positions[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
        else
        {
            CancelInvoke("SpawnCube");
            allCubesSpawned = true;
        }
    }
}

public class WallPatternMovement : MonoBehaviour
{
    private float speed;

    public void Initialize(float moveSpeed)
    {
        speed = moveSpeed;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}