using UnityEngine;

public class BigWall : MonoBehaviour
{
    public float interval = 0.1f;
    public float moveSpeed = 1f; 

    private Transform[] positions;
    private bool allCubesSpawned = false;

    private void Start()
    {
        positions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i);
            positions[i].gameObject.SetActive(false);
        }

        InvokeRepeating("SpawnCube", interval, interval);
    }

    private void Update()
    {
        if (allCubesSpawned && !GetComponent<WallPatternMovement>())
        {
            gameObject.AddComponent<WallPatternMovement>().Initialize(moveSpeed);
            Invoke("DestroyWallPattern", 10f);
        }
    }

    private void DestroyWallPattern()
    {
        Destroy(gameObject);
    }

    private void SpawnCube()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (!positions[i].gameObject.activeSelf)
            {
                positions[i].gameObject.SetActive(true);
                return;
            }
        }
        CancelInvoke("SpawnCube");
        allCubesSpawned = true;
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